static float3 voxelVerts[8] = 
{
    float3(0, 0, 0),
    float3(1, 0, 0),
    float3(1, 1, 0),
    float3(0, 1, 0),
    float3(0, 0, 1),
    float3(1, 0, 1),
    float3(1, 1, 1),
    float3(0, 1, 1)
};

static const float3 faceChecks[6] =
{
    float3(0.0f, 0.0f, -1.0f),
	float3(0.0f, 0.0f, 1.0f),
	float3(0.0f, 1.0f, 0.0f),
	float3(0.0f, -1.0f, 0.0f),
	float3(-1.0f, 0.0f, 0.0f),
	float3(1.0f, 0.0f, 0.0f)
};

static const int voxelTris[6][4] = 
{
    // Back, Front, Top, Bottom, Left, Right

	// 0 1 2 2 1 3
	{0, 3, 1, 2}, // Back Face
	{5, 6, 4, 7}, // Front Face
	{3, 7, 2, 6}, // Top Face
	{1, 5, 0, 4}, // Bottom Face
	{4, 7, 0, 3}, // Left Face
	{1, 2, 5, 6} // Right Face
};

static const float2 voxelUvs[4] =
{

    float2(0.0f, 0.0f),
	float2(0.0f, 1.0f),
	float2(1.0f, 0.0f),
	float2(1.0f, 1.0f)
};