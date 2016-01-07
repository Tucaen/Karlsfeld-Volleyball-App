using System;
using Android.Widget;
using Android.Content.Res;
using Android.Views;
using Android.Util;
using Android.Content;
using Android.Runtime;

namespace VolleyballApp {
	public class WinScrollView : ScrollView {
		public static int maxHeight = 60; // 60dp

		protected WinScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}

		public WinScrollView (Context context) : base(context) {}

		public WinScrollView (Context context, IAttributeSet attrs) : base(context, attrs) {}

		public WinScrollView (Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) {}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
			heightMeasureSpec = MeasureSpec.MakeMeasureSpec(dpToPx(Resources, maxHeight), MeasureSpecMode.AtMost);
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}

		private int dpToPx(Resources res, int dp) {
			return (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, res.DisplayMetrics);
		}
	}
}

