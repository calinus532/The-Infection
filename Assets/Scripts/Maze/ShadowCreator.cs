using System.Reflection; 
using UnityEngine; 
using UnityEngine.Rendering.Universal; 
public class ShadowCreator : MonoBehaviour { 
    private GameObject parent; 
	
	public Vector2[] arrayPathVertices {private set; get;}
	
	private ShadowCreator otherCreator; 
	
	[SerializeField] private string ShadowType; 

	private CompositeCollider2D tilemapCollider; 

	static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance); 
	static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance); 
	static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance); 
	static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D) 
									.Assembly 
									.GetType("UnityEngine.Rendering.Universal.ShadowUtility") 
									.GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static); 
	public void Create() { 
		if (arrayPathVertices == null) arrayPathVertices = new Vector2[0];
        if (GetComponent<CompositeCollider2D>() == null) gameObject.AddComponent<CompositeCollider2D>();
        tilemapCollider = GetComponent<CompositeCollider2D>();
		parent = transform.GetChild(1).gameObject; 

		for(int i = 0; i < transform.parent.gameObject.GetComponentsInChildren<ShadowCreator>().Length; i++) { 
			if(transform.parent.gameObject.GetComponentsInChildren<ShadowCreator>()[i] != GetComponent<ShadowCreator>()) 
				otherCreator = transform.parent.gameObject.GetComponentsInChildren<ShadowCreator>()[i]; 
		} 

		for(int i = 0; i < tilemapCollider.pathCount; i++) { 
			Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)]; 
			tilemapCollider.GetPath(i, pathVertices); 

			if(arrayPathVertices.Length == 0) arrayPathVertices = new Vector2[tilemapCollider.pathCount * pathVertices.Length]; 
			for(int j = 0; j < pathVertices.Length; j++) { 
				pathVertices[j].x = Mathf.Round(pathVertices[j].x * Mathf.Pow(10, 3)) / Mathf.Pow(10, 3); 
				pathVertices[j].y = Mathf.Round(pathVertices[j].y * Mathf.Pow(10, 3)) / Mathf.Pow(10, 3); 
				arrayPathVertices[i * pathVertices.Length + j] = pathVertices[j]; 

				if(otherCreator.arrayPathVertices != null) { 
					if(ShadowType == "Hz") { 
						for(int k = 0; k < otherCreator.arrayPathVertices.Length - 1; k += 2) { 
							int opposite = 0; 
							if(k % 4 == 2) opposite = 1; 
							if(Mathf.Ceil(pathVertices[j].x * 10) == Mathf.Ceil(otherCreator.arrayPathVertices[k].x * 10)) { 
								if(Mathf.Round(pathVertices[j].y * 10) > Mathf.Round(otherCreator.arrayPathVertices[k + opposite].y * 10) 
									&& Mathf.Round(pathVertices[j].y * 10) < Mathf.Round(otherCreator.arrayPathVertices[k + 1 - opposite].y * 10)) { 
										if(k % 4 == 0) CutShadow(i * pathVertices.Length + j, 1); 
										else CutShadow(i * pathVertices.Length + j, -1); 
								} 
							} 
						} 
					} 
					
					if(ShadowType == "Vt") { 
						for(int k = 0; k < otherCreator.arrayPathVertices.Length - 1; k++) { 
							if(k % 4 != 3) { 
								int opposite = 1; 
								if(k % 4 == 0) opposite = 3; 
								if(Mathf.Round(pathVertices[j].x * 10) < Mathf.Round(otherCreator.arrayPathVertices[k].x * 10) 
									&& Mathf.Round(pathVertices[j].x * 10) > Mathf.Round(otherCreator.arrayPathVertices[k + opposite].x * 10)
									&& Mathf.Round(pathVertices[j].y * 10) == Mathf.Round(otherCreator.arrayPathVertices[k].y * 10)) { 
										if(k % 4 == 0) CutShadow(i * pathVertices.Length + j, -1); 
										else CutShadow(i * pathVertices.Length + j, 1); 
								} 
							} 
						} 
					} 
				} 
			} 

			GameObject shadowCaster = new GameObject("shadow_caster_" + i); 
			shadowCaster.transform.parent = parent.transform; 
			shadowCaster.transform.localPosition = Vector3.zero; 
			ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>(); 
			shadowCaster.AddComponent<Shadow>(); 
			
			Vector3[] testPath = new Vector3[pathVertices.Length]; 
			for(int j = 0; j < pathVertices.Length; j++) { 
				testPath[j] = arrayPathVertices[i * pathVertices.Length + j]; 
			} 

			shapePathField.SetValue(shadowCasterComponent, testPath); 
			shapePathHashField.SetValue(shadowCasterComponent, Random.Range(int.MinValue, int.MaxValue)); 
			meshField.SetValue(shadowCasterComponent, new Mesh()); 
			generateShadowMeshMethod.Invoke(shadowCasterComponent,
			new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
		} 
	} 
	
	private void CutShadow(int idx, int sign) { 
		if(ShadowType == "Hz") arrayPathVertices[idx].x -= (0.13f * sign); 

		if(ShadowType == "Vt") arrayPathVertices[idx].y -= (0.13f * sign); 
	} 

	public void deleteShadows() { 
		for(int i = parent.transform.childCount - 1; i >= 0; i--) { 
			Destroy(parent.transform.GetChild(i).gameObject); 
		} 
		arrayPathVertices = new Vector2[0]; 
	} 
} 
