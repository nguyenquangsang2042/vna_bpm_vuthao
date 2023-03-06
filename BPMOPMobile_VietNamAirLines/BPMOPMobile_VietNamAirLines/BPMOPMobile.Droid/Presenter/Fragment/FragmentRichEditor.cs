using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BPMOPMobile.Droid.Class;
using Jp.Wasabeef;
using static Jp.Wasabeef.RichEditor;

namespace BPMOPMobile.Droid.Presenter.Fragment
{
    public class FragmentRichEditor : CustomBaseFragment, IOnTextChangeListener
    {
        private MainActivity _mainAct;
        private LayoutInflater _inflater;
        private View _rootView;
        private RichEditor mEditor;
        private TextView mPreview;
        private bool isChanged;
        private string initText = "";

        public FragmentRichEditor(string initText)
        {
            this.initText = initText;
        }
        public void OnTextChange(string text)
        {
            mPreview.Text = text;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.ViewRichEditor, null);
            _mainAct = (MainActivity)this.Activity;
            if (_mainAct._drawerLayout != null)
            {
                mEditor = _rootView.FindViewById<RichEditor>(Resource.Id.editor_ViewRichEditor);
                mPreview = _rootView.FindViewById<TextView>(Resource.Id.preview_ViewRichEditor);

                mEditor.SetHtml(initText);
                

                mEditor.SetOnTextChangeListener(this);
                mEditor.SetEditorHeight(200);
                mEditor.SetEditorFontSize(22);
                mEditor.SetEditorFontColor(Color.Red);
                mEditor.SetPadding(10, 10, 10, 10);
                mEditor.SetPlaceholder("Insert text here...");

                //mEditor.setEditorBackgroundColor(Color.BLUE);
                //mEditor.setBackgroundColor(Color.BLUE);
                //mEditor.setBackgroundResource(R.drawable.bg);
                //mEditor.setBackground("https://raw.githubusercontent.com/wasabeef/art/master/chip.jpg");
                //mEditor.setInputEnabled(false);



                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_undo).Click += delegate { mEditor.Undo(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_redo).Click += delegate { mEditor.Redo(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_bold).Click += delegate { mEditor.SetBold(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_italic).Click += delegate { mEditor.SetItalic(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_subscript).Click += delegate { mEditor.SetSubscript(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_superscript).Click += delegate { mEditor.SetSuperscript(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_strikethrough).Click += delegate { mEditor.SetStrikeThrough(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_underline).Click += delegate { mEditor.SetUnderline(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading1).Click += delegate { mEditor.SetHeading(1); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading2).Click += delegate { mEditor.SetHeading(2); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading3).Click += delegate { mEditor.SetHeading(3); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading4).Click += delegate { mEditor.SetHeading(4); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading5).Click += delegate { mEditor.SetHeading(5); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_heading6).Click += delegate { mEditor.SetHeading(6); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_txt_color).Click += delegate { mEditor.SetTextColor(isChanged ? Color.Black : Color.Red); isChanged = !isChanged; };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_bg_color).Click += delegate { mEditor.SetTextBackgroundColor(isChanged ? Color.Transparent : Color.Yellow); isChanged = !isChanged; };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_indent).Click += delegate { mEditor.SetIndent(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_outdent).Click += delegate { mEditor.SetOutdent(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_align_left).Click += delegate { mEditor.SetAlignLeft(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_align_center).Click += delegate { mEditor.SetAlignCenter(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_align_right).Click += delegate { mEditor.SetAlignRight(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_blockquote).Click += delegate { mEditor.SetBlockquote(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_bullets).Click += delegate { mEditor.SetBullets(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_numbers).Click += delegate { mEditor.SetNumbers(); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_image).Click += delegate { mEditor.InsertImage("http://www.1honeywan.com/dachshund/image/7.21/7.21_3_thumb.JPG", "dachshund"); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_link).Click += delegate { mEditor.InsertLink("https://github.com/wasabeef", "wasabeef"); };

                _rootView.FindViewById<ImageView>(Resource.Id.img_ViewRichEditor_insert_checkbox).Click += delegate { mEditor.InsertTodo(); };
            }
            return _rootView;
        }

    }
}