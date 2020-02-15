using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.gravityOverride
{
    [Serializable]
    public struct GravityOverrideDisc
    {
        [OnValueChanged("SetupGravity")]
        public bool Face;
        [OnValueChanged("SetupGravity")]
        public bool Borders;

        [SerializeField, ReadOnly]
        private bool _canApplyGravity;
        public bool CanApplyGravity { get { return (_canApplyGravity); } }

        public void SetupGravity()
        {
            _canApplyGravity = Face || Borders;
            Debug.Log("setup gravity of Disc ! " + _canApplyGravity);
        }
    }

    [Serializable]
    public struct GravityOverrideCylinder
    {
        [OnValueChanged("SetupGravity", true)]
        public GravityOverrideDisc Disc1;
        [OnValueChanged("SetupGravity", true)]
        public GravityOverrideDisc Disc2;
        [OnValueChanged("SetupGravity")]
        public bool Trunk;

        [SerializeField, ReadOnly]
        private bool _canApplyGravity;
        public bool CanApplyGravity { get { return (_canApplyGravity); } }

        public GravityOverrideCylinder(GravityOverrideDisc disc1, GravityOverrideDisc disc2, bool trunk)
        {
            Disc1 = disc1;
            Disc2 = disc2;
            Trunk = trunk;

            _canApplyGravity = false;
            SetupGravity();
        }

        public void SetupGravity()
        {
            Disc1.SetupGravity();
            Disc2.SetupGravity();
            _canApplyGravity = Disc1.CanApplyGravity || Disc2.CanApplyGravity || Trunk;
            Debug.Log("setup gravity of cylinder ! " + _canApplyGravity);
        }
    }

    ///
    ///      6 ------------ 7
    ///    / |    3       / |
    ///  5 ------------ 8   |       
    ///  |   |          |   |      
    ///  | 5 |     6    | 2 |     ------8-----  
    ///  |   |   1      |   |                   
    ///  |  2 ----------|-- 3                   
    ///  |/       4     | /     |       3      | 
    ///  1 ------------ 4                       
    ///                                         
    ///          6 ------6----- 5 ------2----- 8 -----10----- 7       -       
    ///          |              |              |              |               
    ///          |              |              |              |               
    ///          5      5       1       1      3       2      11       6       |
    ///          |              |              |              |               
    ///          |              |              |              |               
    ///          2 ------7----- 1 ------4----- 4 ------12---- 3       -
    ///                                         
    ///                                         
    ///                         |       4      |  
    ///                                         
    ///                                         
    ///                           ------9-----  
    [Serializable]
    public struct GravityOverrideCube
    {
        [OnValueChanged("SetupGravity")]  public bool Face1;
        [OnValueChanged("SetupGravity")]  public bool Face2;
        [OnValueChanged("SetupGravity")]  public bool Face3;
        [OnValueChanged("SetupGravity")]  public bool Face4;
        [OnValueChanged("SetupGravity")]  public bool Face5;
        [OnValueChanged("SetupGravity")]  public bool Face6;

        [OnValueChanged("SetupGravity")]  public bool Line1;
        [OnValueChanged("SetupGravity")]  public bool Line2;
        [OnValueChanged("SetupGravity")]  public bool Line3;
        [OnValueChanged("SetupGravity")]  public bool Line4;
        [OnValueChanged("SetupGravity")]  public bool Line5;
        [OnValueChanged("SetupGravity")]  public bool Line6;
        [OnValueChanged("SetupGravity")]  public bool Line7;
        [OnValueChanged("SetupGravity")]  public bool Line8;
        [OnValueChanged("SetupGravity")]  public bool Line9;
        [OnValueChanged("SetupGravity")]  public bool Line10;
        [OnValueChanged("SetupGravity")]  public bool Line11;
        [OnValueChanged("SetupGravity")]  public bool Line12;

        [OnValueChanged("SetupGravity")]  public bool Point1;
        [OnValueChanged("SetupGravity")]  public bool Point2;
        [OnValueChanged("SetupGravity")]  public bool Point3;
        [OnValueChanged("SetupGravity")]  public bool Point4;
        [OnValueChanged("SetupGravity")]  public bool Point5;
        [OnValueChanged("SetupGravity")]  public bool Point6;
        [OnValueChanged("SetupGravity")]  public bool Point7;
        [OnValueChanged("SetupGravity")]  public bool Point8;

        [SerializeField, ReadOnly]
        private bool _canApplyGravity;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace1;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace2;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace3;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace4;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace5;
        [SerializeField, ReadOnly]
        private bool _canApplyGravityBordersAndFace6;
        public bool CanApplyGravity { get { return (_canApplyGravity); } }

        public void SetupGravity()
        {
            _canApplyGravityBordersAndFace1 = Face1 || Line1 || Line2 || Line3 || Line4 || Point1 || Point5 || Point8 || Point4;
            _canApplyGravityBordersAndFace2 = Face2 || Line3 || Line10 || Line11 || Line12 || Point4 || Point8 || Point7 || Point3;
            _canApplyGravityBordersAndFace3 = Face3 || Line6 || Line8 || Line10 || Line2 || Point5 || Point6 || Point7 || Point8;
            _canApplyGravityBordersAndFace4 = Face4 || Line7 || Line4 || Line12 || Line9 || Point2 || Point1 || Point4 || Point3;
            _canApplyGravityBordersAndFace5 = Face5 || Line5 || Line6 || Line1 || Line7 || Point2 || Point6 || Point5 || Point1;
            _canApplyGravityBordersAndFace6 = Face6 || Line11 || Line8 || Line5 || Line9 || Point3 || Point7 || Point6 || Point2;

            _canApplyGravity = _canApplyGravityBordersAndFace1 || _canApplyGravityBordersAndFace2 || _canApplyGravityBordersAndFace3
            || _canApplyGravityBordersAndFace4 || _canApplyGravityBordersAndFace5 || _canApplyGravityBordersAndFace6;
            Debug.Log("setup gravity of cube ! " + _canApplyGravity);
        }
    }
}
