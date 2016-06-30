using System;
namespace MyTransit.Android
{
	public class PreferenceManager
	{
		private static PreferenceManager instance;
		public static PreferenceManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new PreferenceManager();
				}

				return instance;
			}
		}

		private PreferenceManager()
		{
		}
	}
}

