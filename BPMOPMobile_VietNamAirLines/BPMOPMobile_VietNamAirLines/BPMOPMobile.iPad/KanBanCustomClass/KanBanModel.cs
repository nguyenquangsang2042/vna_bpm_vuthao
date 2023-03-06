using System;
using Foundation;
using UIKit;

namespace BPMOPMobile.iPad.KanBanCustomClass
{
    public partial class KanBanModel : NSObject
    {
		public KanBanModel()
		{
		}

		public readonly Guid Identifier = Guid.NewGuid();

		public NSItemProvider ItemProvider
		{
			get
			{
				return new NSItemProvider() ;
			}
		}
		#region Equatable
		public override bool Equals(System.Object obj)
		{
			return ((Photo)obj).Identifier == Identifier;
		}
		public bool Equals(Photo p)
		{
			return p.Identifier == Identifier;
		}
		public override int GetHashCode()
		{
			return Identifier.GetHashCode();
		}
		#endregion

		public UIImage image;
		public string ID { get; set; }
        public string Title { get; set; }
        public NSIndexPath CardIndexPath { get; set; }

       
    }
}
