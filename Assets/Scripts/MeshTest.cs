using UnityEngine;
using System.Linq;

public class MeshTest : MonoBehaviour
{
	private string[] circleOfFifths = {"C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"};
	private Vector2[] circlePos = new Vector2[12];
	private string[] midiNames = {"C","Db","D","Eb","E","F","Gb","G","Ab","A","Bb","B"};
	private float radius = 3.56f;
	private Mesh mesh;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
    private void Start()
    {
        for (int i = 0; i < 12; i++) {
			float angle = findAngle(circleOfFifths[i]);
			circlePos[i] = new Vector2(radius * Mathf.Sin(Mathf.Deg2Rad * angle), radius * Mathf.Cos(Mathf.Deg2Rad * angle));
		}
    }

	public void drawMesh(Vector2[] vertices2D){
		var vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);

		// Use the triangulator to get indices for creating triangles
		var triangulator = new Triangulator(vertices2D);
		var indices =  triangulator.Triangulate();
		
		// Generate a color for each vertex
		var colors = Enumerable.Range(0, vertices3D.Length)
			.Select(i => Random.ColorHSV())
			.ToArray();

		// Create the mesh
		if (mesh == null) {
			mesh = new Mesh {
				vertices = vertices3D,
				triangles = indices,
				colors = colors,
			};
		} else {
			mesh.Clear();
			mesh.vertices = vertices3D;
			mesh.triangles = indices;
			mesh.colors = colors;
		}
		// var mesh = new Mesh {
		// 	vertices = vertices3D,
		// 	triangles = indices,
		// 	colors = colors
		// };
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		// Set up game object with mesh;
		if (meshRenderer == null) {
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			//meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
			meshRenderer.material = new Material(Shader.Find("Custom/blinking"));
		}
		
		if (meshFilter == null) {
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		
		meshFilter.mesh = mesh;
	}

	public void clearMesh() {
		this.mesh.Clear();
	}
	
	void Update()
	{
		// if (Input.GetKeyDown("e")){
		// 	var vertices2D = new Vector2[] {
		// 		circlePos[0],
		// 		circlePos[1],
		// 		circlePos[4],
		// 		circlePos[5]
		// 	};

		// 	var vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);
	
		// 	// Use the triangulator to get indices for creating triangles
		// 	var triangulator = new Triangulator(vertices2D);
		// 	var indices =  triangulator.Triangulate();
			
		// 	// Generate a color for each vertex
		// 	var colors = Enumerable.Range(0, vertices3D.Length)
		// 		.Select(i => Random.ColorHSV())
		// 		.ToArray();

		// 	// Create the mesh
		// 	var mesh = new Mesh {
		// 		vertices = vertices3D,
		// 		triangles = indices,
		// 		colors = colors
		// 	};
			
		// 	mesh.RecalculateNormals();
		// 	mesh.RecalculateBounds();
	
		// 	// Set up game object with mesh;
		// 	var meshRenderer = gameObject.AddComponent<MeshRenderer>();
		// 	meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
			
		// 	var filter = gameObject.AddComponent<MeshFilter>();
		// 	filter.mesh = mesh;
		// }
	}
	private float findAngle(string noteName){
		float angle = 0;
		for (int i = 0; i < circleOfFifths.Length; i++) {
			if (circleOfFifths[i].Equals(noteName)) {
				angle = i * 30;
			}
		}
		return angle;
	}
	private string noteNum2Name(int noteNum){
		return this.midiNames[noteNum % 12];
	}
}
