using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public ComputeShader chunkShader;
    //int PopulateVoxelMap;
    int AddVoxelDataToChunk;

    static  Vector3Int dimms = new (8,8,8);
    [SerializeField]
    static  int dimmsVolume = dimms.x * dimms.y * dimms.z;
    public static int maxVertexCount = dimmsVolume * 6 * 4;
    readonly int maxTriangleCount = dimmsVolume * 6 * 2;
    Mesh mesh;

    //ComputeBuffer voxelMap;
    GraphicsBuffer vertexBuffer;
    GraphicsBuffer normalBuffer;
    GraphicsBuffer texcoordBuffer;
    GraphicsBuffer indexBuffer;

    // DEBUG
    ComputeBuffer vertexDebugBuffer;
    ComputeBuffer indexDebugBuffer;
    ComputeBuffer idDebugBuffer;
    [SerializeField]
    Vector3[] vertexDebug;
    [SerializeField]
    Vector3Int[] indexDebug;
    [SerializeField]
    uint[] idDebug;


    void Start()
    {
        //PopulateVoxelMap = chunkShader.FindKernel("PopulateVoxelMap");
        AddVoxelDataToChunk = chunkShader.FindKernel("AddVoxelDataToChunk");

        AllocateBuffers();
        AllocateMesh();
    }

    void AllocateBuffers()
    {        
        //_voxelMap = new ComputeBuffer(dimmsVolume, 4, ComputeBufferType.Structured);
        //chunkShader.SetInts("_dimms", dimms.x, dimms.y, dimms.z);
        chunkShader.SetInt("_dimmX", dimms.x);
        chunkShader.SetInt("_dimmY", dimms.y);
        chunkShader.SetInt("_dimmZ", dimms.z);

        // Debug buffers
        vertexDebugBuffer = new ComputeBuffer(maxVertexCount*3, sizeof(float));
        indexDebugBuffer = new ComputeBuffer(maxTriangleCount, sizeof(uint)*3);
        vertexDebug = new Vector3[maxVertexCount];
        indexDebug = new Vector3Int[maxTriangleCount];

        idDebugBuffer = new ComputeBuffer(dimmsVolume, sizeof(uint));
        idDebug = new uint[dimmsVolume];
        Debug.Log("dimmsVolume: "+dimmsVolume);
    }
     
    void RelaseBuffers()
    {
        //_voxelMap.Dispose();
    }

    void AllocateMesh()
    {
        mesh = new Mesh
        {
            name = "ChunkMesh"
        };

        mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
        mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;

        var vertexLayout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, 
                VertexAttributeFormat.Float32, 3, stream:0),            
            new VertexAttributeDescriptor(VertexAttribute.Normal,
                VertexAttributeFormat.Float32, 3, stream:1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0,
                VertexAttributeFormat.Float32, 2, stream:2),
        };

        mesh.SetVertexBufferParams(maxVertexCount, vertexLayout);
        mesh.SetIndexBufferParams(maxTriangleCount*3, IndexFormat.UInt32);

        //??????????????????????????????????????????????????????????????????????????????????????????
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, maxVertexCount),
                        MeshUpdateFlags.DontRecalculateBounds);

        vertexBuffer ??= mesh.GetVertexBuffer(0);
        normalBuffer ??= mesh.GetVertexBuffer(1);
        texcoordBuffer ??= mesh.GetVertexBuffer(2);
        indexBuffer ??= mesh.GetIndexBuffer();
    }

    void RelaseMesh()
    {
        vertexBuffer?.Dispose();
        normalBuffer?.Dispose();
        texcoordBuffer?.Dispose();
        indexBuffer?.Dispose();
        vertexDebugBuffer?.Dispose();
        indexDebugBuffer?.Dispose();
        idDebugBuffer?.Dispose();
        Object.Destroy(mesh);
    }

    void DrawMesh()
    {
        uint xc, yc, zc;
        chunkShader.GetKernelThreadGroupSizes(AddVoxelDataToChunk, out xc, out yc, out zc);
        //int x = (dimms.x + (int)xc - 1) / (int)xc;
        //int y = (dimms.y + (int)yc - 1) / (int)yc;
        //int z = (dimms.z + (int)zc - 1) / (int)zc;
        int x = dimms.x / (int)xc;
        int y = dimms.y / (int)yc;
        int z = dimms.z / (int)zc;

        vertexBuffer ??= mesh.GetVertexBuffer(0);
        normalBuffer ??= mesh.GetVertexBuffer(1);
        texcoordBuffer ??= mesh.GetVertexBuffer(2);
        indexBuffer ??= mesh.GetIndexBuffer();

        chunkShader.SetInt("_dimmX", dimms.x);
        chunkShader.SetInt("_dimmY", dimms.y);
        chunkShader.SetInt("_dimmZ", dimms.z);

        chunkShader.SetBuffer(AddVoxelDataToChunk, "_vertexBuffer", vertexBuffer);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_normalBuffer", normalBuffer);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_texcoordBuffer", texcoordBuffer);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_indexBuffer", indexBuffer);
        //chunkShader.SetBuffer(AddVoxelDataToChunk, "_voxelMap", voxelMap);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_vertexDebugBuffer", vertexDebugBuffer);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_indexDebugBuffer", indexDebugBuffer);
        chunkShader.SetBuffer(AddVoxelDataToChunk, "_idDebugBuffer", idDebugBuffer);
        chunkShader.Dispatch(AddVoxelDataToChunk,x,y,z);

        GetComponent<MeshFilter>().sharedMesh = mesh;
        vertexBuffer.GetData(vertexDebug);
        indexBuffer.GetData(indexDebug);
        idDebugBuffer.GetData(idDebug);
    }

    void Update()
    {
        DrawMesh();
    }

    void OnDestroy()
    {
        RelaseBuffers();
        RelaseMesh();
    }
}

