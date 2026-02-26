using Godot;
using System;

public partial class Worldgen1 : StaticBody3D
{
	[Export] public Mesh BaseMesh;
	[Export] public float NoiseScale = 0.5f;
	[Export] public float NoiseStrength = 5.0f;

	private MeshInstance3D _meshInstance;
	private CollisionShape3D _collisionShape;
	private ArrayMesh _arrayMesh;
	private FastNoiseLite _noise;

	public Texture2D GrassTexture;
	public Texture2D ForestTexture;
	public Texture2D RockTexture;

	public int WorldSeed = 0;

	public override void _Ready()
	{
		_meshInstance = new MeshInstance3D();
		AddChild(_meshInstance);

		_collisionShape = new CollisionShape3D();
		AddChild(_collisionShape);

		_noise = new FastNoiseLite();
		_noise.Seed = WorldSeed;
		_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
		_noise.FractalOctaves = 5;
		_noise.FractalLacunarity = 2.0f;
		_noise.FractalGain = 0.5f;

		_arrayMesh = ConvertToArrayMesh(BaseMesh);
		ApplyNoiseDeformation();
		_meshInstance.Mesh = _arrayMesh;
		ApplyMaterial();
		_collisionShape.Shape = _arrayMesh.CreateTrimeshShape();
	}

	private void ApplyNoiseDeformation()
	{
		var arrays = _arrayMesh.SurfaceGetArrays(0);
		var vertices = (Vector3[])arrays[(int)Mesh.ArrayType.Vertex];

		for (int i = 0; i < vertices.Length; i++)
		{
			float noiseValue = _noise.GetNoise2D(
				(Position.X + vertices[i].X) * NoiseScale,  // ✅ erst addieren, dann skalieren
				(Position.Z + vertices[i].Z) * NoiseScale
			);
			vertices[i].Y += noiseValue * NoiseStrength;
		}

		arrays[(int)Mesh.ArrayType.Vertex] = vertices;

		// ArrayMesh neu aufbauen mit neuen Vertices
		_arrayMesh.ClearSurfaces();
		_arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
	}

	private ArrayMesh ConvertToArrayMesh(Mesh sourceMesh)
	{
		var arrayMesh = new ArrayMesh();

		for (int i = 0; i < sourceMesh.GetSurfaceCount(); i++)
		{
			var arrays = sourceMesh.SurfaceGetArrays(i);
			arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		}

		return arrayMesh;
	}

	private void ApplyMaterial()
	{
		var shader = GD.Load<Shader>("res://Scripts/terrain.gdshader");
		var mat = new ShaderMaterial();
		mat.Shader = shader;
		
		mat.SetShaderParameter("grass_texture",  GrassTexture);
		mat.SetShaderParameter("forest_texture", ForestTexture);
		mat.SetShaderParameter("rock_texture",   RockTexture);
		mat.SetShaderParameter("rock_height",    5.0f);
		mat.SetShaderParameter("blend_range",    2.0f);
		mat.SetShaderParameter("texture_scale",  0.5f);
		
		_meshInstance.MaterialOverride = mat;
	}
	private int HashPosition(Vector3 pos)
	{
		return (int)(pos.X * 1000) ^ (int)(pos.Z * 1000);
	}


}
