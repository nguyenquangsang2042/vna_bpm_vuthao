using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Bean;
using BPMOPMobile.Class;
using BPMOPMobile.Droid.Core.Common;
using SQLite;

namespace BPMOPMobile.Droid.Core.Controller
{
    public class ControllerComment : ControllerBase
    {

        // Query Comment
        public string _queryComment = @"SELECT * FROM BeanComment WHERE ResourceId = '{0}'";
        public string _queryDeleteOldComment = @"DELETE FROM BeanComment WHERE ResourceId = '{0}'";
        public string _queryUpdateIsLiked = @"UPDATE BeanComment Set IsLiked ={0} And LikeCount = {1} WHERE ResourceId = '{2}' "; // update lại sau khi bấm like / unlike

        //Query DetailWorkflow
        //public string _queryUpdateCommentChangeWFItem = @"UPDATE BeanWorkflowItem Set CommentChanged='{0}' WHERE ID={1}";
        //public string _queryUpdateIsChangWFItem = @"UPDATE BeanWorkflowItem Set IsChange={0} WHERE ID={1}";
        public string _queryUpdateCommentChangeWFItem = @"UPDATE BeanWorkflowItem Set CommentChanged='{0}' AND IsChange={1} WHERE ID ={2}";


        /// <summary>
        /// Hàm để update lại SQlite Sau khi bấm Like / Unlike Comment
        /// </summary>
        /// <param name="_otherResourceID">Resouce ID chứ ko phải commentID</param>
        /// <param name="_IsLiked">Chú ý: Giá trị sau khi bấm nút</param>
        /// <param name="_LikeCount">Chú ý: Like count sau khi bấm nút</param>
        public void UpdateIsLikedComment(string _OtherResourceID, bool _IsLiked, int _LikeCount, SQLiteConnection conn = null)
        {
            try
            {
                if (conn == null)
                    conn = new SQLiteConnection(CmmVariable.M_DataPath);

                //_queryUpdateIsLiked = @"UPDATE BeanComment Set IsLiked ={0} And LikeCount = {1} WHERE ResourceId = '{2}' "; 
                string _query = String.Format(_queryUpdateIsLiked, _IsLiked == true ? 1 : 0, _LikeCount, _OtherResourceID);
                conn.Execute(_query);
            }
            catch (Exception ex)
            {
#if DEBUG
                CmmDroidFunction.WriteTrackingError(this.GetType().Name, "UpdateIsLikedComment", ex);
#endif
            }
            finally
            {
                conn.Close();
            }

        }

    }
}