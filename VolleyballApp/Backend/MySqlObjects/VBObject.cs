using System;
using Android.OS;
using Object = Java.Lang.Object;

namespace VolleyballApp {
	public abstract class VBObject : Object, IParcelable {
		public VBObject() {
		}

		public abstract void WriteToParcel(Parcel dest, ParcelableWriteFlags flags);

		public int DescribeContents() {
			return 0;
		}

		public class MyParcelableCreator<T> : Object, IParcelableCreator where T : Object, new() {

			private readonly Func<Parcel, T> _createFunc;

			public MyParcelableCreator(Func<Parcel, T> createFromParcelFunc) {
				_createFunc = createFromParcelFunc;
			}

			public Object CreateFromParcel (Parcel source) {
				return _createFunc(source);
			}

			public Object[] NewArray (int size) {
				return new Object[size];
			}
		}
	}
}

