﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using g3;

namespace f3
{
    //
    // fCamera wraps a Camera for frame3Sharp. The idea is that eventually we
    //  will be able to "replace" Camera with something else, ie non-Unity stuff.
    //
    // implicit cast operators allow transparent conversion between Camera and fCamera
    //
    public class fCamera
    {
        Camera camera;


        public fCamera(Camera go)
        {
            this.camera = go;
        }


        public fGameObject GameObject()
        {
            return new fGameObject(this.camera.gameObject);
        }


        public void SetName(string name)
        {
            camera.name = name;
        }
        public string GetName()
        {
            return camera.name;
        }


        public bool HasChildren()
        {
            return camera.transform.childCount > 0;
        }
        public System.Collections.IEnumerable Children()
        {
            for (int k = 0; k < camera.transform.childCount; ++k)
                yield return camera.transform.GetChild(k).gameObject;
        }

        public void SetParent(fGameObject parentGO, bool bKeepWorldPosition = false)
        {
            if (parentGO == null)
                camera.transform.parent = null;
            else
                camera.transform.SetParent(((GameObject)parentGO).transform, bKeepWorldPosition);
        }


        public void SetPosition(Vector3f vPosition)
        {
            camera.transform.position = vPosition;
        }
        public Vector3f GetPosition()
        {
            return camera.transform.position;
        }

        public void SetLocalPosition(Vector3f vPosition)
        {
            camera.transform.localPosition = vPosition;
        }
        public Vector3f GetLocalPosition()
        {
            return camera.transform.localPosition;
        }

        public void SetLocalScale(Vector3f vScale)
        {
            camera.transform.localScale = vScale;
        }
        public void SetLocalScale(float fScale)
        {
            camera.transform.localScale = fScale * Vector3f.One; 
        }
        public Vector3f GetLocalScale()
        {
            return camera.transform.localScale;
        }


        public Frame3f GetWorldFrame()
        {
            return new Frame3f(camera.transform.position, camera.transform.rotation);
        }
        public void SetWorldFrame(Frame3f f)
        {
            camera.transform.position = f.Origin;
            camera.transform.rotation = f.Rotation;
        }


        public Vector3f Forward()
        {
            return camera.transform.forward;
        }
        public Vector3f Up()
        {
            return camera.transform.up;
        }
        public Vector3f Right()
        {
            return camera.transform.right;
        }



        public CameraManipulator GetManipulator()
        {
            return camera.gameObject.GetComponent<CameraManipulator>();
        }


        public Vector3f GetTarget()
        {
            CameraTarget t = camera.gameObject.GetComponent<CameraTarget>();
            return t.TargetPoint;
        }
        public void SetTarget(Vector3f newTarget)
        {
            CameraTarget t = camera.gameObject.GetComponent<CameraTarget>();
            t.TargetPoint = newTarget;
        }
        public void SetTargetVisible(bool bVisible)
        {
            CameraTarget t = camera.gameObject.GetComponent<CameraTarget>();
            t.ShowTarget = bVisible;
        }


        // we attach this below
        public CameraManipulator Manipulator()
        {
            return camera.gameObject.GetComponent<CameraManipulator>();
        }



        public static implicit operator UnityEngine.Camera(fCamera cam)
        {
            return cam.camera;
        }
        public static implicit operator fCamera(UnityEngine.Camera cam)
        {
            return new fCamera(cam);
        }
    }
}
