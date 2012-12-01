using UnityEngine;
using System.Collections;

public class G3 : MonoBehaviour {

	public enum FunctionOption {
		Linear,
		Cubic,
		Parabola,
		Sine,
		Ripple
	}
	public UnityEngine.Vector2 origin;
	public FunctionOption function;
	
	private delegate float FunctionDelegate (InputData d);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Hyperbolic,
		Sine,
		Ripple
	};
	
	public int resolution = 10;
	
	public float timestep = 1;
	
	public class InputData{
		public Vector3 p;
		public Vector2 o;
		public float t;
		public InputData (Vector3 p,
		Vector2 o,
		float t)
		{
			this.t = t;
			this.o = o;
			this.p = p;
		}
	}
	private int currentResolution;
	private ParticleSystem.Particle[] points;
	public bool Center = false;
	
	void CreatePoints ()
	{
		if(resolution < 2){
			resolution = 2;
		}
		else if(resolution > 40){
			resolution = 40;
		}
		
		currentResolution = resolution;
		points = new ParticleSystem.Particle[resolution * resolution * resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for(int x = 0; x < resolution; ++x){
			for(int z = 0; z < resolution; ++z){ 
				Vector3 p = new Vector3(x*increment, 0f, z*increment);
				points[i].position = p;
				points[i].color = new Color(p.x, 0f, p.z);
				points[i].size = 0.1f;
				++i;
			}
		}
	}
	
	
	
	// Use this for initialization
	void Start () {
		CreatePoints ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Center) {
			origin.x = origin.y = .5f;
			Center = false;
		}
		if (currentResolution != resolution) CreatePoints();
		FunctionDelegate f = functionDelegates[(int)function];
		float t = Time.timeSinceLevelLoad;
		
		for(int i = 0; i < points.Length; ++i){
			Vector3 p = points[i].position;
			p.y = f (new InputData(p, origin, t*timestep));
			points[i].position = p;
			Color c = points[i].color;
			c.g = p.y;
			points[i].color = c;
		}
		particleSystem.SetParticles(points, points.Length);
	}

	private static float Linear (InputData d)
	{
		return d.p.x;
	}
	
	private static float Exponential (InputData d){
		return d.p.x * d.p.x;
	}
	
	private static float Hyperbolic (InputData d){
		d.p.x = 2f * d.p.x - 1f;
		d.p.z = 2f * d.p.z - 1f;
		return 1f - d.p.x * d.p.x * d.p.z * d.p.z;
	}
	
	private static float Sine (InputData d){
		return 0.50f + 
//				0.25f * Mathf.Sin(4 * Mathf.PI * p.x + 4 * t) * Mathf.Sin(2 * Mathf.PI * p.z + t) + 
//				0.10f * Mathf.Cos(3 * Mathf.PI * p.x + 5 * t) * Mathf.Cos(5 * Mathf.PI * p.z + 3 *t) +
//				0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
			Mathf.Sin(d.p.x*10 +d.t*5) *.1f + Mathf.Sin(d.p.z*10+d.t*5) *.1f;
	}
	
	private static float Ripple (InputData d){
		float squareRadius = (d.p.x - d.o.x ) * (d.p.x - d.o.x ) + (d.p.z - d.o.y ) * (d.p.z - d.o.y );
		return 0.5f + Mathf.Sin(15 * Mathf.PI * squareRadius - 2f * d.t) / (2f + 100f * squareRadius);
	}
}
