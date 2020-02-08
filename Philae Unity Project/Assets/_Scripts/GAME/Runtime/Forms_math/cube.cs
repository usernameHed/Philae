using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    private Vector3 _position;
    private Quaternion _rotation;
    private Vector3 _localScale;

    private Matrix4x4 _cubeMatrix;
    private CubePoints _myCube;

    private Vector3 _closestPoint;

    public struct CubePoints
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 P4;
        public Vector3 P5;
        public Vector3 P6;
        public Vector3 P7;
        public Vector3 P8;
    }

    private VectorPosition GetVectorPositionFrom2Points(Vector3 p1, Vector3 p2)
    {
        Vector3 direction = new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
        return new VectorPosition(p1, direction);
    }
    private VectorPosition AddVectors(VectorPosition vector1, VectorPosition vector2)
    {
        return vector1;
    }
    private VectorPosition InversVector(VectorPosition vector)
    {
        float distX = vector.Force.x - vector.Position.x;
        float distY = vector.Force.y - vector.Position.y;
        float distZ = vector.Force.z - vector.Position.z;

        Vector3 newForce = new Vector3(vector.Position.x - distX, vector.Position.y - distY, vector.Position.z - distZ);
        return new VectorPosition(vector.Position, newForce);
    }
    private VectorPosition subVectors(VectorPosition vector1, VectorPosition vector2)
    {
        return AddVectors(vector1, InversVector(vector2));
    }
    private Vector3 getClosestPoint(CubePoints cube, Vector3 point)
    {
        VectorPosition VectPos1 = GetVectorPositionFrom2Points(cube.P1, point);

        VectorPosition Vect = GetVectorPositionFrom2Points(cube.P1, point);

        VectorPosition AdjVect1 = GetVectorPositionFrom2Points(cube.P2, point);
        VectorPosition AdjVect2 = GetVectorPositionFrom2Points(cube.P4, point);
        VectorPosition AdjVect3 = GetVectorPositionFrom2Points(cube.P5, point);

        

        return point;
    }


    // Start is called before the first frame update
    void Start()
    {
        _cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale);
        Vector3 size = Vector3.one;

        _myCube.P1 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
        _myCube.P2 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
        _myCube.P3 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
        _myCube.P4 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

        _myCube.P5 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
        _myCube.P6 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, size.z) * 0.5f));
        _myCube.P7 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((size) * 0.5f));
        _myCube.P8 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, size.y, -size.z) * 0.5f));

        GameObject ship = GameObject.Find("ship");
        if (ship)
        {
            Vector3 shipPosition = ship.transform.position;
            _closestPoint = getClosestPoint(_myCube, shipPosition);

            ExtDrawGuizmos.DebugWireSphere(_closestPoint, 2.5f, 10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
