﻿using System;
using System.Collections.Generic;
using g3;

namespace f3
{

    public static class IOStrings
    {
        public static readonly string SceneVersion = "SceneVersion";
        //public static readonly string CurrentSceneVersion = "1.0";    // initial verison, flat attribute lists inside SOs
        public static readonly string CurrentSceneVersion = "1.1";      // added structs


        // top-level object names
        public static readonly string Scene = "Scene";
        public static readonly string SceneObject = "SceneObject";

        // structs are collections of attributes inside scene/sceneobject
        public static readonly string Struct = "Struct";
        public const string StructType = "Type";
        public const string StructIdentifier = "ID";
        public const string BinaryMeshStruct = "BinaryUEncodedMesh";
        public const string AsciiMeshStruct = "AsciiMesh";
        public const string TransformStruct = "Transform";
        public const string MaterialStruct = "Material";
        public const string KeyframeListStruct = "KeyframeList";
        public const string KeyframeStruct = "Keyframe";

        // standard scene object attributes
        public static readonly string ASOType = "SOType";
        public static readonly string ASOName = "SOName";

        // scene object type names
        public static readonly string TypeUnknown = "Unknown";
        public static readonly string TypeCylinder = "Cylinder";
        public static readonly string TypeBox = "Box";
        public static readonly string TypeSphere = "Sphere";
        public static readonly string TypePivot = "Pivot";
        public static readonly string TypeMesh = "Mesh";
        public static readonly string TypeGroup = "Group";
        public static readonly string TypeMeshReference = "MeshReference";
        public static readonly string TypePolyCurve = "PolyCurve";
        public static readonly string TypePolyTube = "PolyTube";

        // NOTE: attribute prefix characters are used to determine type when parsing:
        //   sAttribName => string
        //   iAttribName => integer
        //   fAttribName => float
        //   vAttribName => Vector3f
        //   uAttribname => Vector2f
        //   qAttribName => Quaternionf
        //   cAttribName => Colorf
        //   zd2 => list of 2-element doubles (eg Vector2d)
        //   zd3 => list of 3-element doubles (eg Vector3d)
        //   x is same as z, but uuencoded binary buffer, instead of text elements

        // transform attributes
        public static readonly string APosition = "vPosition";
        public static readonly string AOrientation = "qOrientation";
        public static readonly string AScale = "vScale";

        // scene object specific attributes
        public static readonly string AWidth = "fWidth";
        public static readonly string AHeight = "fHeight";
        public static readonly string ADepth = "fDepth";
        public static readonly string ARadius = "fRadius";
        public static readonly string AStartPoint = "vStartPoint";
        public static readonly string AEndPoint = "vEndPoint";

        public static readonly string AReferencePath = "sReferencePath";
        public static readonly string ARelReferencePath = "sRelReferencePath";

        // for polycuve
        public static readonly string APolyCurve3 = "zd3Curve";
        public static readonly string APolyCurveClosed = "bCurveClosed";
        public static readonly string APolygon2 = "zd2Polygon";

        // for mesh
        public static readonly string AMeshVertices3 = "zd3MeshVertices";
        public static readonly string AMeshVertices3Binary = "xd3MeshVertices";
        public static readonly string AMeshNormals3 = "zf3MeshNormals";
        public static readonly string AMeshNormals3Binary = "xd3MeshNormals";
        public static readonly string AMeshColors3 = "zf3MeshColors";
        public static readonly string AMeshColors3Binary = "xf3MeshColors";
        public static readonly string AMeshUVs2 = "zf2MeshUVs";
        public static readonly string AMeshUVs2Binary = "xf2MeshUVs";
        public static readonly string AMeshTriangles = "zi3MeshTriangles";
        public static readonly string AMeshTrianglesBinary = "xi3MeshTriangles";

        // material attributes
        public static readonly string AMaterialName = "sMaterialName";
        public static readonly string AMaterialType = "eMaterialType";
        public static readonly string AMaterialType_Standard = "Standard";
        public static readonly string AMaterialType_Transparent = "Transparent";
        public static readonly string AMaterialRGBColor = "cMaterialRGBColor";

        // keyframe attributes
        public const string ATime = "fTime";
        public const string ATimeRange = "uTimeRange";

    }



    public interface IOutputStream
    {
        // top-level scene
        void BeginScene(string version);
        void EndScene();

        // scene contains scene objects
        void BeginSceneObject();
        void EndSceneObject();

        // scene object contains attribute
        void AddAttribute(string sName, string sValue, bool bInline = false);
        void AddAttribute(string sName, float fValue, bool bInline = false);
        void AddAttribute(string sName, int nValue, bool bInline = false);
        void AddAttribute(string sName, bool bValue, bool bInline = false);
        void AddAttribute(string sName, Vector2f vValue, bool bInline = false);
        void AddAttribute(string sName, Vector3f vValue, bool bInline = false);
        void AddAttribute(string sName, Quaternionf qValue, bool bInline = false);
        void AddAttribute(string sName, Colorf cColor, bool bInline = false);
        void AddAttribute(string sName, IEnumerable<Vector3d> vVec);
        void AddAttribute(string sName, IEnumerable<Vector3f> vVec);
        void AddAttribute(string sName, IEnumerable<Vector3i> vVec);
        void AddAttribute(string sName, IEnumerable<Index3i> vIdx);
        void AddAttribute(string sName, IEnumerable<Vector2d> vVec);
        void AddAttribute(string sName, IEnumerable<Vector2f> vVec);
        void AddAttribute(string sName, byte[] buffer);

        // a struct contains a group of attributes. Optional Identifier can be used to
        // differentiate between multiple structs of same Type in a single object.
        void BeginStruct(string sType, string sIdentifier = "");
        void EndStruct();
    }



    public delegate void InputStream_NodeHandler();
    public delegate void InputStream_StructHandler(string sType, string sIdentifier);
    public delegate void InputStream_AttributeHandler(string sName, string sValue);
    public interface IInputStream
    {
        event InputStream_NodeHandler OnBeginScene;
        event InputStream_NodeHandler OnEndScene;
        event InputStream_NodeHandler OnBeginSceneObject;
        event InputStream_NodeHandler OnEndSceneObject;

        event InputStream_StructHandler OnBeginStruct;
        event InputStream_StructHandler OnEndStruct;

        event InputStream_AttributeHandler OnAttribute;

        void Restore();
    }


    public delegate void SerializeMessageHandler(string sMessage);


    // list of values, possibly with nested TypedAttribSets
    public class TypedAttribSet
    {
        public string Type = "";        // only set for Structs, ie nested attrib sets
        public string Identifier = "";  // differentiate between same-type Structs
        public Dictionary<string, object> Pairs = new Dictionary<string, object>();

        public bool ContainsKey(string s)
        {
            return Pairs.ContainsKey(s);
        }
        public object this[string key] {
            get { return Pairs[key]; }
        }
    }


    // custom store/restore functions for SOs. 
    public delegate bool SOEmitSerializationFunc(SceneSerializer serializer, IOutputStream o, SceneObject so);
    public delegate SceneObject SOBuildFunc(SOFactory factory, FScene scene, TypedAttribSet attributes);


    public interface ISceneObjectFactory
    {
        SceneObject Build(FScene s, SceneSerializer serializer, TypedAttribSet attributes);
        event SerializeMessageHandler OnMessage;
    }


    public class SceneSerializer
    {
        public SceneSerializer()
        {
        }

        public ISceneObjectFactory SOFactory { get; set; }

        public string TargetFilePath { get; set; }

        /*
         * Store
         */


        public void Store(IOutputStream o, FScene scene)
        {
            o.BeginScene(IOStrings.CurrentSceneVersion);

            foreach (SceneObject so in scene.SceneObjects) {
                if (so.IsTemporary)
                    continue;

                o.BeginSceneObject();

                string typeIdentifier = so.Type.identifier;

                // serialize custom types if client has registered a serializer
                bool bEmitted = false;
                if ( scene.TypeRegistry.ContainsType(typeIdentifier ) ) {
                    SOEmitSerializationFunc emitter = scene.TypeRegistry.FindSerializer(typeIdentifier);
                    if (emitter != null)
                        bEmitted = emitter(this, o, so);
                }

                if (bEmitted == false) {
                    // otherwise fall back to default handlers. 
                    // Currently these are extension methods in SceneSerializerEmitTypes.cs
                    if (so is CylinderSO)
                        this.Emit(o, so as CylinderSO);
                    else if (so is BoxSO)
                        this.Emit(o, so as BoxSO);
                    else if (so is SphereSO)
                        this.Emit(o, so as SphereSO);
                    else if (so is PivotSO)
                        this.Emit(o, so as PivotSO);
                    else if (so is MeshSO)
                        this.Emit(o, so as MeshSO);
                    else if (so is MeshReferenceSO)
                        this.Emit(o, so as MeshReferenceSO);

                    else if (so is PolyTubeSO)
                        this.Emit(o, so as PolyTubeSO);
                    else if (so is PolyCurveSO)
                        this.Emit(o, so as PolyCurveSO);

                    else if (so is TransformableSO)
                        this.Emit(o, so as TransformableSO);
                    else
                        this.EmitUnknown(o, so);
                }

                o.EndSceneObject();
            }


            o.EndScene();
        }


        // fallback for completely unknown types
        public void EmitUnknown(IOutputStream o, SceneObject so)
        {
            o.AddAttribute(IOStrings.ASOType, IOStrings.TypeUnknown);
            o.AddAttribute(IOStrings.ASOName, so.Name);
        }



        /*
         * Restore
         */

        FScene activeScene;     // scene we are restoring into


        enum RestoreState
        {
            NoState,
            InScene,
            InSceneObject,
            InStruct,
            Done
        }
        RestoreState eState = RestoreState.NoState;

        List<RestoreState> state_stack = new List<RestoreState>();
        void PushState(RestoreState newState)
        {
            // verify valid transitions??
            state_stack.Add(eState);
            eState = newState;
        }
        void PopState()
        {
            if (eState == RestoreState.NoState)
                throw new Exception("SceneSerializer.PopState: aready in NoState! must be unbalanced push/pop somewhere!");
            eState = RestoreState.NoState;
            if ( state_stack.Count > 0 ) {
                eState = state_stack[state_stack.Count - 1];
                state_stack.RemoveAt(state_stack.Count - 1);
            }
        }


 
        TypedAttribSet CurAttribs;

        List<TypedAttribSet> attrib_stack = new List<TypedAttribSet>();
        void PushAttribSet(string sType = "", string sIdentifier = "")
        {
            TypedAttribSet s = new TypedAttribSet() { Type = sType };
            if (CurAttribs != null)
                attrib_stack.Add(CurAttribs);
            CurAttribs = s;
        }
        void PopAttribSet()
        {
            if (CurAttribs == null)
                throw new Exception("SceneSerializer.PopAttribSet: no attrib set active! must be unbalanced push/pop somewhere!");

            CurAttribs = null;
            if ( attrib_stack.Count > 0 ) {
                CurAttribs = attrib_stack[attrib_stack.Count - 1];
                attrib_stack.RemoveAt(attrib_stack.Count - 1);
            }
        }




        public void Restore(IInputStream i, FScene s)
        {
            activeScene = s;

            i.OnBeginScene += Restore_OnBeginScene;
            i.OnEndScene += Restore_OnEndScene;
            i.OnBeginSceneObject += Restore_OnBeginSceneObject;
            i.OnEndSceneObject += Restore_OnEndSceneObject;
            i.OnBeginStruct += Restore_OnBeginStruct;
            i.OnEndStruct += Restore_OnEndStruct;
            i.OnAttribute += Restore_OnAttribute;

            i.Restore();
        }


        void Restore_OnBeginScene()
        {
            if (eState != RestoreState.NoState)
                throw new FormatException("[Serializer] not in correct state for BeginScene");
            PushState(RestoreState.InScene);
        }
        void Restore_OnEndScene()
        {
            if (eState != RestoreState.InScene)
                throw new FormatException("[Serializer] not in correct state for EndScene");
            PopState();
            PushState(RestoreState.Done);
        }
        void Restore_OnBeginSceneObject()
        {
            if (eState != RestoreState.InScene)
                throw new FormatException("[Serializer] not in correct state for BeginSceneObject");
            PushState(RestoreState.InSceneObject);
            PushAttribSet();
        }
        void Restore_OnEndSceneObject()
        {
            if (eState != RestoreState.InSceneObject)
                throw new FormatException("[Serializer] not in correct state for EndSceneObject");

            SceneObject so = SOFactory.Build(activeScene, this, CurAttribs);
            if (so != null) {
                activeScene.AddSceneObject(so);
            }

            PopAttribSet();
            PopState();
        }

        void Restore_OnBeginStruct(string sType, string sIdentifier)
        {
            PushState(RestoreState.InStruct);
            PushAttribSet(sType, sIdentifier);
        }
        void Restore_OnEndStruct(string sType, string sIdentifier)
        {
            TypedAttribSet structAttribs = CurAttribs;
            PopAttribSet();
            if (CurAttribs == null)
                throw new FormatException("Serializer.Restore_OnEndStruct: no valid attrib set to add struct to!");

            string idString = sType;
            if (sIdentifier.Length > 0)
                idString += ":" + sIdentifier;
            CurAttribs.Pairs[idString] = structAttribs;

            PopState();
        }



        void Restore_OnAttribute(string sName, string sValue)
        {
            if (eState != RestoreState.InSceneObject && eState != RestoreState.InStruct)
                throw new FormatException("[Serializer] not in correct state for OnAttrib");

            char[] delimiterChars = { ' ' };

            if (sName == IOStrings.Struct) {


            } else if (sName[0] == 'i') {
                int iValue = 0;
                int.TryParse(sValue, out iValue);
                CurAttribs.Pairs[sName] = iValue;

            } else if (sName[0] == 'f') {
                float fValue = 0;
                float.TryParse(sValue, out fValue);
                CurAttribs.Pairs[sName] = fValue;

            } else if (sName[0] == 'b') {
                bool bValue = true;
                if (sValue.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                    bValue = false;
                CurAttribs.Pairs[sName] = bValue;

            } else if (sName[0] == 'v') {
                float x = 0, y = 0, z = 0;
                string[] values = sValue.Split(delimiterChars);
                if (values.Length == 3) {
                    float.TryParse(values[0], out x);
                    float.TryParse(values[1], out y);
                    float.TryParse(values[2], out z);
                }
                CurAttribs.Pairs[sName] = new Vector3f(x, y, z);

            } else if (sName[0] == 'u') {
                float x = 0, y = 0;
                string[] values = sValue.Split(delimiterChars);
                if (values.Length == 2) {
                    float.TryParse(values[0], out x);
                    float.TryParse(values[1], out y);
                }
                CurAttribs.Pairs[sName] = new Vector2f(x, y);

            } else if (sName[0] == 'q') {
                float x = 0, y = 0, z = 0, w = 0;
                string[] values = sValue.Split(delimiterChars);
                if (values.Length == 4) {
                    float.TryParse(values[0], out x);
                    float.TryParse(values[1], out y);
                    float.TryParse(values[2], out z);
                    float.TryParse(values[3], out w);
                }
                CurAttribs.Pairs[sName] = new Quaternionf(x, y, z, w);

            } else if (sName[0] == 'c') {
                float r = 0, g = 0, b = 0, a = 0;
                string[] values = sValue.Split(delimiterChars);
                if (values.Length == 4) {
                    float.TryParse(values[0], out r);
                    float.TryParse(values[1], out g);
                    float.TryParse(values[2], out b);
                    float.TryParse(values[3], out a);
                }
                CurAttribs.Pairs[sName] = new Colorf(r, g, b, a);


            } else if (sName[0] == 'z') {
                if (sName[1] == 'd' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3d(sValue);
                else if (sName[1] == 'f' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3f(sValue);
                else if (sName[1] == 'i' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3i(sValue);
                else if (sName[1] == 'd' && sName[2] == '2')
                    CurAttribs.Pairs[sName] = restore_list2d(sValue);
                else if (sName[1] == 'f' && sName[2] == '2')
                    CurAttribs.Pairs[sName] = restore_list2f(sValue);
                else
                    DebugUtil.Warning("[SceneSerializer.Restore_OnAttribute] - unknown array format {0}", sName);

            } else if (sName[0] == 'x') {
                if (sName[1] == 'd' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3d_binary(sValue);
                else if (sName[1] == 'f' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3f_binary(sValue);
                else if (sName[1] == 'i' && sName[2] == '3')
                    CurAttribs.Pairs[sName] = restore_list3i_binary(sValue);
                //else if (sName[1] == 'd' && sName[2] == '2')
                //    CurAttribs.Pairs[sName] = restore_list2d_binary(sValue);
                else if (sName[1] == 'f' && sName[2] == '2')
                    CurAttribs.Pairs[sName] = restore_list2f_binary(sValue);
                else
                    DebugUtil.Warning("[SceneSerializer.Restore_OnAttribute] - unknown binary format {0}", sName);


            } else {
                CurAttribs.Pairs[sName] = sValue;
            }


        }


        VectorArray3d restore_list3d(String valueString)
        {
            string[] values = valueString.Split(' ');
            int N = values.Length / 3;
            VectorArray3d v = new VectorArray3d(N);
            for (int i = 0; i < N; ++i) {
                double x = 0, y = 0, z = 0;
                double.TryParse(values[3 * i], out x);
                double.TryParse(values[3 * i + 1], out y);
                double.TryParse(values[3 * i + 2], out z);
                v.Set(i, x, y, z);
            }
            return v;
        }

        VectorArray3f restore_list3f(String valueString)
        {
            string[] values = valueString.Split(' ');
            int N = values.Length / 3;
            VectorArray3f v = new VectorArray3f(N);
            for (int i = 0; i < N; ++i) {
                float x = 0, y = 0, z = 0;
                float.TryParse(values[3 * i], out x);
                float.TryParse(values[3 * i + 1], out y);
                float.TryParse(values[3 * i + 2], out z);
                v.Set(i, x, y, z);
            }
            return v;
        }

        VectorArray3i restore_list3i(String valueString)
        {
            string[] values = valueString.Split(' ');
            int N = values.Length / 3;
            VectorArray3i v = new VectorArray3i(N);
            for (int i = 0; i < N; ++i) {
                int x = 0, y = 0, z = 0;
                int.TryParse(values[3 * i], out x);
                int.TryParse(values[3 * i + 1], out y);
                int.TryParse(values[3 * i + 2], out z);
                v.Set(i, x, y, z);
            }
            return v;
        }


        VectorArray2d restore_list2d(String valueString)
        {
            string[] values = valueString.Split(' ');
            int N = values.Length / 2;
            VectorArray2d v = new VectorArray2d(N);
            for (int i = 0; i < N; ++i) {
                double x = 0, y = 0;
                double.TryParse(values[2 * i], out x);
                double.TryParse(values[2 * i + 1], out y);
                v.Set(i, x, y);
            }
            return v;
        }

        VectorArray2f restore_list2f(String valueString)
        {
            string[] values = valueString.Split(' ');
            int N = values.Length / 2;
            VectorArray2f v = new VectorArray2f(N);
            for (int i = 0; i < N; ++i) {
                float x = 0, y = 0;
                float.TryParse(values[2 * i], out x);
                float.TryParse(values[2 * i + 1], out y);
                v.Set(i, x, y);
            }
            return v;
        }




        VectorArray3d restore_list3d_binary(String valueString)
        {
            char[] str = valueString.ToCharArray();
            byte[] buffer = Convert.FromBase64CharArray(str, 0, str.Length);
            int sz = sizeof(double);
            int Nvals = buffer.Length / sz;
            int Nvecs = Nvals / 3;
            VectorArray3d v = new VectorArray3d(Nvecs);
            for (int i = 0; i < Nvecs; i++) {
                double x = BitConverter.ToDouble(buffer, (3 * i) * sz);
                double y = BitConverter.ToDouble(buffer, (3 * i + 1) * sz);
                double z = BitConverter.ToDouble(buffer, (3 * i + 2) * sz);
                v.Set(i, x, y, z);
            }
            return v;
        }


        VectorArray3f restore_list3f_binary(String valueString)
        {
            char[] str = valueString.ToCharArray();
            byte[] buffer = Convert.FromBase64CharArray(str, 0, str.Length);
            int sz = sizeof(float);
            int Nvals = buffer.Length / sz;
            int Nvecs = Nvals / 3;
            VectorArray3f v = new VectorArray3f(Nvecs);
            for (int i = 0; i < Nvecs; i++) {
                float x = BitConverter.ToSingle(buffer, (3 * i) * sz);
                float y = BitConverter.ToSingle(buffer, (3 * i + 1) * sz);
                float z = BitConverter.ToSingle(buffer, (3 * i + 2) * sz);
                v.Set(i, x, y, z);
            }
            return v;
        }


        VectorArray2f restore_list2f_binary(String valueString)
        {
            char[] str = valueString.ToCharArray();
            byte[] buffer = Convert.FromBase64CharArray(str, 0, str.Length);
            int sz = sizeof(float);
            int Nvals = buffer.Length / sz;
            int Nvecs = Nvals / 2;
            VectorArray2f v = new VectorArray2f(Nvecs);
            for (int i = 0; i < Nvecs; i++) {
                float x = BitConverter.ToSingle(buffer, (2 * i) * sz);
                float y = BitConverter.ToSingle(buffer, (2 * i + 1) * sz);
                v.Set(i, x, y);
            }
            return v;
        }


        VectorArray3i restore_list3i_binary(String valueString)
        {
            char[] str = valueString.ToCharArray();
            byte[] buffer = Convert.FromBase64CharArray(str, 0, str.Length);
            int sz = sizeof(int);
            int Nvals = buffer.Length / sz;
            int Nvecs = Nvals / 3;
            VectorArray3i v = new VectorArray3i(Nvecs);
            for (int i = 0; i < Nvecs; i++) {
                int x = BitConverter.ToInt32(buffer, (3 * i) * sz);
                int y = BitConverter.ToInt32(buffer, (3 * i + 1) * sz);
                int z = BitConverter.ToInt32(buffer, (3 * i + 2) * sz);
                v.Set(i, x, y, z);
            }
            return v;
        }


    }

}
