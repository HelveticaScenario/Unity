using UnityEngine;

public class Grapher3 : MonoBehaviour {
	
	public enum FunctionOption {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple
	}
	
	private delegate float FunctionDelegate (Vector3 p, float t);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple
	};
	
	public bool absolute;
	public float threshold = 0.5f;
	public FunctionOption function;
	public int resolution = 10;
	
	private int currentResolution;
	private ParticleSystem.Particle[] points;
	
	void Start () {
		CreatePoints();
	}
	
	private void CreatePoints () {
		if(resolution < 2){
			resolution = 2;
		}
		else if(resolution > 30){
			resolution = 30;
		}
		currentResolution = resolution;
		points = new ParticleSystem.Particle[resolution * resolution * resolution];
		float increment = 1f / (resolution - 1);
		int i = 0;
		for(int x = 0; x < resolution; x++){
			for(int z = 0; z < resolution; z++){
				for(int y = 0; y < resolution; y++){
					Vector3 p = new Vector3(x, y, z) * increment;
					points[i].position = p;
					points[i].color = new Color(p.x, p.y, p.z);
					points[i++].size = 0.1f;
				}
			}
		}
	}
	
	void Update () {
		if(currentResolution != resolution){
			CreatePoints();
		}
		FunctionDelegate f = functionDelegates[(int)function];
		float t = Time.timeSinceLevelLoad;
		if(absolute){
			for(int i = 0; i < points.Length; i++){
				Color c = points[i].color;
				c.a = f(points[i].position, t) >= threshold ? 1f : 0f;
				points[i].color = c;
			}
		}
		else{
			for(int i = 0; i < points.Length; i++){
				Color c = points[i].color;
				c.a = f(points[i].position, t);
				points[i].color = c;
			}
		}
		particleSystem.SetParticles(points, points.Length);
	}
	
	private static float Linear (Vector3 p, float t) {
		return 1f - p.x - p.y - p.z + 0.5f * Mathf.Sin(t);
	}
	
	private static float Exponential (Vector3 p, float t) {
		return 1f - p.x * p.x - p.y * p.y - p.z * p.z + 0.5f * Mathf.Sin(t);
	}
	
	private static float Parabola (Vector3 p, float t){
		p.x = 2f * p.x - 1f;
		p.z = 2f * p.z - 1f;
		return 1f - p.x * p.x - p.z * p.z + 0.5f * Mathf.Sin(t);
	}
	
	private static float Sine (Vector3 p, float t){
		float x = Mathf.Sin(2 * Mathf.PI * p.x);
		float y = Mathf.Sin(2 * Mathf.PI * p.y);
		float z = Mathf.Sin(2 * Mathf.PI * p.z + (p.y > 0.5f ? t : -t));
		return x * x * y * y * z * z;
	}
	
	private static float Ripple (Vector3 p, float t){
		float squareRadius =
			(p.x - 0.5f) * (p.x - 0.5f) +
			(p.y - 0.5f) * (p.y - 0.5f) +
			(p.z - 0.5f) * (p.z - 0.5f);
		return Mathf.Sin(4 * Mathf.PI * squareRadius - 2f * t);
	}
}
