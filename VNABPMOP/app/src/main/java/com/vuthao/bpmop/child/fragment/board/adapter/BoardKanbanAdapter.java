package com.vuthao.bpmop.child.fragment.board.adapter;

import android.app.Activity;
import android.content.Context;
import android.content.res.ColorStateList;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.custom.boardview.BoardAdapter;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.model.custom.BoardKanBan;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.child.fragment.board.VarsChildBoard;

import java.util.ArrayList;
import java.util.Arrays;

import de.hdodenhof.circleimageview.CircleImageView;
import io.realm.Realm;

public class BoardKanbanAdapter extends BoardAdapter {
    private final Activity activity;
    private final ArrayList<BoardKanBan> _lstBoardKanBan;
    private final Realm realm;

    public BoardKanbanAdapter(Context context, Activity activity, ArrayList<BoardKanBan> _lstBoardKanBan) {
        super(context);
        this.activity = activity;
        this._lstBoardKanBan = _lstBoardKanBan;
        realm = new RealmController().getRealm();

        for (int i = 0; i < _lstBoardKanBan.size(); i++) {
            if (_lstBoardKanBan.get(i).getLstAppBase() != null) {
                for (int j = 0; j < _lstBoardKanBan.get(i).getLstAppBase().size(); j++) {
                    if (_lstBoardKanBan.get(i).getLstAppBase().get(j).getResourceCategoryId() != 16) {
                        String workflowId = Functions.share.getWorkflowItemIDByUrl(_lstBoardKanBan.get(i).getLstAppBase().get(j).getItemUrl());
                        WorkflowFollow follow = realm.where(WorkflowFollow.class)
                                .equalTo("WorkflowItemId", Integer.parseInt(workflowId))
                                .findFirst();
                        if (follow != null) {
                            _lstBoardKanBan.get(i).getLstAppBase().get(j).setFollow(follow.getStatus());
                        }
                    }
                }
            }
        }
    }

    public ArrayList<BoardKanBan> setFollow(String workflowId, boolean isFollow) {
        for (int i = 0; i < _lstBoardKanBan.size(); i++) {
            if (_lstBoardKanBan.get(i).getLstAppBase() != null) {
                for (int j = 0; j < _lstBoardKanBan.get(i).getLstAppBase().size(); j++) {
                    if (_lstBoardKanBan.get(i).getLstAppBase().get(j).getResourceCategoryId() != 16) {
                        String wId = Functions.share.getWorkflowItemIDByUrl(_lstBoardKanBan.get(i).getLstAppBase().get(j).getItemUrl());
                       if (wId.equals(workflowId)) {
                           _lstBoardKanBan.get(i).getLstAppBase().get(j).setFollow(isFollow ? 1 : 0);
                       }
                    }
                }
            }
        }
        return _lstBoardKanBan;
    }

    public ArrayList<BoardKanBan> getListData() {
        return _lstBoardKanBan;
    }

    public AppBase getItem(int columnPosition, int itemPosition) {
        return _lstBoardKanBan.get(columnPosition).getLstAppBase().get(itemPosition);
    }

    @Override
    public int getColumnCount() {
        return _lstBoardKanBan.size();
    }

    @Override
    public int getItemCount(int column_position) {
        if (column_position > _lstBoardKanBan.size() - 1) {
            return 0;
        }

        if (_lstBoardKanBan.get(column_position).getLstAppBase() != null &&
                _lstBoardKanBan.get(column_position).getLstAppBase().size() > 0) {
            return _lstBoardKanBan.get(column_position).getLstAppBase().size();
        }
        return 0;
    }

    @Override
    public int maxItemCount(int column_position) {
        return Integer.MAX_VALUE;
    }

    @Override
    public Object createHeaderObject(int column_position) {
        return null;
    }

    @Override
    public Object createFooterObject(int column_position) {
        return null;
    }

    @Override
    public Object createItemObject(int column_position, int item_position) {
        return null;
    }

    @Override
    public boolean isColumnLocked(int column_position) {
        return true;
    }

    @Override
    public boolean isItemLocked(int column_position) {
        //Không được thao tác trên cột Phê duyệt và Từ chối
        return column_position >= _lstBoardKanBan.size() - 2;
    }

    @Override
    public View createItemView(Context context, Object header_object, Object item, int column_position, int item_position) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_board_detail_group_recy_child, null);
        BoardKanbanChildHolder holder = new BoardKanbanChildHolder();
        holder._lnAll = view.findViewById(R.id.ln_ItemBoardDetailGroup_RecyChild_All);
        holder._lnData = view.findViewById(R.id.ln_ItemBoardDetailGroup_RecyChild_Data);
        holder._cardAll = view.findViewById(R.id.card_ItemBoardDetailGroup_RecyChild_All);
        holder._tvDate = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_Date);
        holder._imgSubCribe = view.findViewById(R.id.img_ItemBoardDetailGroup_RecyChild_Subcribe);
        holder._relaAvatar = view.findViewById(R.id.rela_ItemBoardDetailGroup_RecyChild_Avatar);
        holder._tvAvatar = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_Avatar);
        holder._imgAvatar = view.findViewById(R.id.img_ItemBoardDetailGroup_RecyChild_Avatar);
        holder._tvTitle = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_Title);
        holder._tvCountAttach = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_CountAttach);
        holder._tvCountComment = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_CountComment);
        holder._relaAvatar2 = view.findViewById(R.id.rela_ItemBoardDetailGroup_RecyChild_Avatar2);
        holder._tvAvatar2 = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_Avatar2);
        holder._imgAvatar2 = view.findViewById(R.id.img_ItemBoardDetailGroup_RecyChild_Avatar2);
        holder._tvCountPeople = view.findViewById(R.id.tv_ItemBoardDetailGroup_RecyChild_CountPeople);

        AppBase base = _lstBoardKanBan.get(column_position).getLstAppBase().get(item_position);
        if (item_position == _lstBoardKanBan.get(column_position).getLstAppBase().size() - 1) {
            holder._lnAll.setBackgroundResource(R.drawable.textgrayboard_bot2corner);
        } else {
            holder._lnAll.setBackgroundResource(R.color.clGrayNavigator);
        }

        if (base != null) {
            if (Arrays.stream(VarsChildBoard.ApprovedListID).anyMatch(r -> r == base.getStatusGroup())) {
                holder._cardAll.setCardBackgroundColor(ContextCompat.getColor(activity, R.color.clStatusGreen));
            } else if (Arrays.stream(VarsChildBoard.RejectedListID).anyMatch(r -> r == base.getStatusGroup())) {
                holder._cardAll.setCardBackgroundColor(ContextCompat.getColor(activity, R.color.clStatusRed));
            } else {
                holder._cardAll.setCardBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
            }

            if ((base.getAppFlg() & 1) > 0) {
                holder._cardAll.setCardBackgroundColor(ContextCompat.getColor(activity, R.color.clStatusBlue));
            } else if ((base.getAppFlg() & 2) > 0) {
                holder._cardAll.setCardBackgroundColor(ContextCompat.getColor(activity, R.color.clBoardOrangeRequestInfo));
            }

            if (!Functions.isNullOrEmpty(base.getDueDate())) {
                holder._tvDate.setText(Functions.share.formatDateLanguage(base.getDueDate()));
            } else {
                holder._tvDate.setText("");
            }

            if (base.isFollow() == 1) {
                holder._imgSubCribe.setVisibility(View.VISIBLE);
                holder._imgSubCribe.setImageResource(R.drawable.icon_ver2_star_checked);
            } else {
                holder._imgSubCribe.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(base.getCreatedBy())) {
                String value = base.getCreatedBy().split(",")[0];
                User user = realm.where(User.class)
                        .equalTo("ID", value.toLowerCase())
                        .findFirst();
                if (user != null) {
                    if (!Functions.isNullOrEmpty(user.getImagePath())) {
                        holder._tvAvatar.setVisibility(View.INVISIBLE);
                        holder._imgAvatar.setVisibility(View.VISIBLE);
                        ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + user.getImagePath(), holder._imgAvatar);
                    } else {
                        holder._tvAvatar.setVisibility(View.VISIBLE);
                        holder._imgAvatar.setVisibility(View.INVISIBLE);

                        holder._tvAvatar.setText(Functions.share.getAvatarName(user.getFullName()));
                        holder._tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(user.getFullName())));
                    }
                } else {
                    holder._tvAvatar.setVisibility(View.VISIBLE);
                    holder._imgAvatar.setVisibility(View.INVISIBLE);

                    holder._tvAvatar.setText(Functions.share.getAvatarName(value));
                    holder._tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(value)));
                }
            } else {
                holder._tvAvatar.setVisibility(View.INVISIBLE);
                holder._imgAvatar.setVisibility(View.INVISIBLE);
            }

            if (!Functions.isNullOrEmpty(base.getContent())) {
                holder._tvTitle.setText(base.getContent());
            } else {
                holder._tvTitle.setText("");
            }

            if (base.getFileCount() > 0) {
                Functions.share.setFormatItemCount(holder._tvCountAttach, base.getFileCount(), "", "");
            } else {
                holder._tvCountAttach.setText("0");
            }

            if (base.getCommentCount() > 0) {
                Functions.share.setFormatItemCount(holder._tvCountComment, base.getCommentCount(), "", "");
            } else {
                holder._tvCountComment.setText("0");
            }

            if (!Functions.isNullOrEmpty(base.getAssignedTo())) {
                String[] arr = base.getAssignedTo().split(",");

                if (arr.length > 1) {
                    holder._tvCountPeople.setText("+" + (arr.length - 1));
                } else {
                    holder._tvCountPeople.setText("");
                }

                User user = realm.where(User.class)
                        .equalTo("ID", arr[0].toLowerCase())
                        .findFirst();
                if (user != null) {
                    if (!Functions.isNullOrEmpty(user.getImagePath())) {
                        holder._tvAvatar2.setVisibility(View.INVISIBLE);
                        holder._imgAvatar2.setVisibility(View.VISIBLE);
                        ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + user.getImagePath(), holder._imgAvatar2);
                    } else {
                        holder._tvAvatar2.setVisibility(View.VISIBLE);
                        holder._imgAvatar2.setVisibility(View.INVISIBLE);

                        holder._tvAvatar2.setText(Functions.share.getAvatarName(user.getFullName()));
                        holder._tvAvatar2.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(user.getFullName())));
                    }
                } else {
                    holder._tvAvatar2.setVisibility(View.VISIBLE);
                    holder._imgAvatar2.setVisibility(View.INVISIBLE);

                    holder._tvAvatar2.setText(Functions.share.getAvatarName(arr[0]));
                    holder._tvAvatar2.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(arr[0])));
                }
            }
        }

        return view;
    }

    @Override
    public View createHeaderView(Context context, Object header_object, int column_position) {
        header_object = LayoutInflater.from(context).inflate(R.layout.item_board_group_detail_library, null);
        View view = (View) header_object;
        BoardKanbanColumnHolder holder = new BoardKanbanColumnHolder();
        holder._lnAll = view.findViewById(R.id.ln_ItemBoardDetailGroupLibrary_All);
        holder._tvName = view.findViewById(R.id.tv_ItemBoardDetailGroupLibrary_Title);
        holder._tvNoData = view.findViewById(R.id.tv_ItemBoardDetailGroupLibrary_Child_NoData);

        holder._tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));

        if (_lstBoardKanBan.get(column_position).getLstAppBase() != null && _lstBoardKanBan.get(column_position).getLstAppBase().size() > 0) {
            holder._lnAll.setBackgroundResource(R.drawable.textgrayboard_top2corner);
            holder._tvNoData.setVisibility(View.GONE);
        } else {
            holder._lnAll.setBackgroundResource(R.drawable.textgrayboard_4corner);
            holder._tvNoData.setVisibility(View.VISIBLE);
        }

        WorkflowStepDefine workflowStepDefine = _lstBoardKanBan.get(column_position).getItemStepDefine();
        if (workflowStepDefine != null) {
            if (!Functions.isNullOrEmpty(workflowStepDefine.getTitle())) {
                holder._tvName.setText(workflowStepDefine.getTitle());
            } else {
                holder._tvName.setText("");
            }
        }

        return (View) header_object;
    }

    @Override
    public View createFooterView(Context context, Object footer_object, int column_position) {
        return null;
    }

    private static class BoardKanbanColumnHolder {
        LinearLayout _lnAll;
        TextView _tvName;
        TextView _tvNoData;
    }

    private static class BoardKanbanChildHolder {
        LinearLayout _lnAll;
        LinearLayout _lnData;
        CardView _cardAll;
        TextView _tvDate;
        ImageView _imgSubCribe;
        RelativeLayout _relaAvatar;
        TextView _tvAvatar;
        CircleImageView _imgAvatar;
        TextView _tvTitle;
        TextView _tvCountAttach;
        TextView _tvCountComment;
        RelativeLayout _relaAvatar2;
        TextView _tvAvatar2;
        CircleImageView _imgAvatar2;
        TextView _tvCountPeople;
    }
}
