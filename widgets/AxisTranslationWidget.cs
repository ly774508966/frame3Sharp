﻿using System;
using UnityEngine;
using g3;

namespace f3
{
	//
	// this Widget implements translation constrained to an axis
	// 
	public class AxisTranslationWidget : Standard3DWidget
    {
        // scaling distance along axis multiplied by this value. 
        // By default click-point stays under cursor (ie direct manipulation), but 
        // you can slow it down by setting this to return a smaller value.
        // Also you can use this to compensate for global scene scaling (hence the function)
        public Func<float> TranslationScaleF = () => { return 1.0f; };

		int nTranslationAxis;

		public AxisTranslationWidget(int nFrameAxis)
		{
			nTranslationAxis = nFrameAxis;
		}

		// stored frames from target used during click-drag interaction
		Frame3f translateFrameL;		// local-spaace frame
		Frame3f translateFrameW;		// world-space frame
		Vector3 translateAxisW;		// world translation axis (redundant...)

		// computed values during interaction
		Frame3f raycastFrame;		// camera-facing plane containing translateAxisW
		float fTranslateStartT;		// start T-value along translateAxisW

		public override bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit)
		{
			// save local and world frames
			translateFrameL = target.GetLocalFrame (CoordSpace.ObjectCoords);
			translateFrameW = target.GetLocalFrame (CoordSpace.WorldCoords);
			translateAxisW = translateFrameW.GetAxis (nTranslationAxis);

			// save t-value of closest point on translation axis, so we can find delta-t
			Vector3 vWorldHitPos = hit.hitPos;
			fTranslateStartT = Distance.ClosestPointOnLineT(
				translateFrameW.Origin, translateAxisW, vWorldHitPos);

            // construct plane we will ray-intersect with in UpdateCapture()
            Vector3 makeUp = Vector3.Cross(Camera.main.transform.forward, translateAxisW).normalized;
            Vector3 vPlaneNormal = Vector3.Cross(makeUp, translateAxisW).normalized;
            raycastFrame = new Frame3f(vWorldHitPos, vPlaneNormal);

            return true;
		}

		public override bool UpdateCapture(ITransformable target, Ray worldRay)
		{
			// ray-hit with plane that contains translation axis
			Vector3 planeHit = raycastFrame.RayPlaneIntersection(worldRay.origin, worldRay.direction, 2);

			// figure out new T-value along axis, then our translation update is delta-t
			float fNewT = Distance.ClosestPointOnLineT (translateFrameW.Origin, translateAxisW, planeHit);
			float fDeltaT = (fNewT - fTranslateStartT);
            fDeltaT *= TranslationScaleF();

            // construct new frame translated along axis (in local space)
            Frame3f newFrame = translateFrameL;
			newFrame.Origin += fDeltaT * translateFrameL.GetAxis(nTranslationAxis);

			// update target
			target.SetLocalFrame (newFrame, CoordSpace.ObjectCoords);

			return true;
		}

        public override bool EndCapture(ITransformable target)
        {
            return true;
        }
    }
}

