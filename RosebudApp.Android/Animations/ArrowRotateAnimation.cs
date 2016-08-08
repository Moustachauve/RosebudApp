using System;
using Android.Views.Animations;
namespace RosebudAppAndroid
{
	public static class ArrowRotateAnimation
	{
		public static RotateAnimation GetAnimation(float startRotation, float degreeRotation) {
			RotateAnimation animation = new RotateAnimation(startRotation, startRotation - 180f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
			animation.Interpolator = new LinearInterpolator();
			animation.FillAfter = true;
			animation.FillEnabled = true;
			animation.Duration = 300;

			return animation;
		}
	}
}

