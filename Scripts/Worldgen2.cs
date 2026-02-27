using Godot;
using System;

public partial class Worldgen2 : Node
{
	[Export] public Mesh BaseMesh;
	[Export] public int ChunkRadius = 2;
	[Export] public float ChunkSize = 10.0f;
	[Export] public Texture2D GrassTexture;
	[Export] public Texture2D ForestTexture;
	[Export] public Texture2D RockTexture;

	private int _worldSeed;

	public override void _Ready()
	{
		_worldSeed = DatabaseManager.Instance.LoadOrCreateWorldSeed();
		GD.Print("WorldSeed: " + _worldSeed);
		SpawnChunks(Vector3.Zero);
	}

	private void SpawnChunks(Vector3 center)
	{
		for (int x = -ChunkRadius; x <= ChunkRadius; x++)
		{
			for (int z = -ChunkRadius; z <= ChunkRadius; z++)
			{
				Vector3 chunkPosition = new Vector3(
					center.X + x * ChunkSize,
					0,
					center.Z + z * ChunkSize
				);
				SpawnChunk(chunkPosition);
			}
		}
	}

	private void SpawnChunk(Vector3 position)
	{
		var chunk = new Worldgen1();
		chunk.BaseMesh = BaseMesh;
		chunk.GrassTexture = GrassTexture;
		chunk.ForestTexture = ForestTexture;
		chunk.RockTexture = RockTexture;
		chunk.Position = position;
		AddChild(chunk);

		DatabaseManager.Instance.SaveObject(chunk, "TerrainChunk");
	}

	public partial class WorldGen : Node3D
	{
		[Export] public Mesh BaseMesh;
		[Export] public Texture2D GrassTexture;
		[Export] public Texture2D ForestTexture;
		[Export] public Texture2D RockTexture;
		[Export] public int ChunkRadius = 2;
		[Export] public float ChunkSize = 10.0f;
	}
}
