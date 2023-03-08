package com.vuthao.bpmop.child.fragment.board.presenter;

import android.app.Activity;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.NetworkUtil;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiController;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.model.custom.BoardKanBan;
import com.vuthao.bpmop.base.model.custom.ButtonAction;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.model.dynamic.ViewRow;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.child.fragment.board.VarsChildBoard;
import com.vuthao.bpmop.child.fragment.board.adapter.BoardKanbanAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.stream.Stream;

import io.realm.Realm;
import io.realm.RealmResults;
import io.realm.Sort;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChildAppKanbanPresenter {
    private final Realm realm;
    private final ChildAppKanbanListener listener;

    public interface ChildAppKanbanListener {
        void OnSubmitSuccess();

        void OnBackAction(WorkflowItem workflowItem, ButtonAction buttonAction);

        void OnSubmitErr(String err);

        void OnEndDragSuccess(AppBase app, int originalPosition, boolean isNext);

        void OnEndDragErr(String err, int originalColumn);
    }

    public ChildAppKanbanPresenter(ChildAppKanbanListener listener) {
        this.listener = listener;
        realm = new RealmController().getRealm();
    }

    public ArrayList<WorkflowStepDefine> getListStepDefine(int workflowId, int _ApprovedStepID, int _RejectedStepID) {
        RealmResults<WorkflowStepDefine> results = realm.where(WorkflowStepDefine.class)
                .equalTo("WorkflowID", workflowId)
                .findAll();

        ArrayList<WorkflowStepDefine> lstStepDefine = new ArrayList<>(results);
        WorkflowStepDefine workflowStepDefine = new WorkflowStepDefine();
        workflowStepDefine.setWorkflowStepDefineID(_ApprovedStepID);
        workflowStepDefine.setTitle(Functions.share.getTitle("TEXT_APPROVED", "Đã phê duyệt"));
        lstStepDefine.add(workflowStepDefine);

        workflowStepDefine = new WorkflowStepDefine();
        workflowStepDefine.setWorkflowStepDefineID(_RejectedStepID);
        workflowStepDefine.setTitle(Functions.share.getTitle("TEXT_REJECT", "Từ chối"));
        lstStepDefine.add(workflowStepDefine);

        return lstStepDefine;
    }

    public ArrayList<BoardKanBan> setListBoardKanBan(Workflow workflow, ArrayList<WorkflowStepDefine> lstStepDefine, int _ApprovedStepID, int _RejectedStepID) {
        if (lstStepDefine.isEmpty()) {
            return new ArrayList<>();
        }
        ArrayList<BoardKanBan> items = new ArrayList<>();
        RealmResults<AppBase> results;
        for (WorkflowStepDefine stepDefine : lstStepDefine) {
            if (stepDefine.getWorkflowStepDefineID() == _ApprovedStepID) {
                results = realm.where(AppBase.class)
                        .equalTo("WorkflowId", workflow.getWorkflowID())
                        .and()
                        .notEqualTo("ResourceCategoryId", 16)
                        .and()
                        .in("StatusGroup", VarsChildBoard.ApprovedListID)
                        .sort("Created", Sort.DESCENDING)
                        .findAll();
            } else if (stepDefine.getWorkflowStepDefineID() == _RejectedStepID) {
                results = realm.where(AppBase.class)
                        .equalTo("WorkflowId", workflow.getWorkflowID())
                        .and()
                        .notEqualTo("ResourceCategoryId", 16)
                        .and()
                        .in("StatusGroup", VarsChildBoard.RejectedListID)
                        .sort("Created", Sort.DESCENDING)
                        .findAll();
            } else {
                Integer[] both = Stream.concat(Arrays.stream(VarsChildBoard.ApprovedListID)
                        , Arrays.stream(VarsChildBoard.RejectedListID))
                        .toArray(Integer[]::new);

                results = realm.where(AppBase.class)
                        .equalTo("WorkflowId", workflow.getWorkflowID())
                        .and().equalTo("Step", stepDefine.getStep())
                        .and()
                        .notEqualTo("ResourceCategoryId", 16)
                        .and()
                        .not().in("StatusGroup", both)
                        .sort("Created", Sort.DESCENDING)
                        .findAll();
            }

            BoardKanBan boardKanBan = new BoardKanBan();
            boardKanBan.setItemStepDefine(stepDefine);
            boardKanBan.setLstAppBase(new ArrayList<>(results));
            items.add(boardKanBan);
        }
        return items;
    }

    public ArrayList<BoardKanBan> search(ArrayList<BoardKanBan> kanBans, String charText) {
        ArrayList<BoardKanBan> items = new ArrayList<>();
        for (BoardKanBan kanBan : kanBans) {
            BoardKanBan boardKanBan = new BoardKanBan();
            boardKanBan.setItemStepDefine(kanBan.getItemStepDefine());
            ArrayList<AppBase> apps = new ArrayList<>();

            for (AppBase search : kanBan.getLstAppBase()) {
                if (Functions.removeAccent(search.getContent()).toLowerCase()
                        .contains(Functions.removeAccent(charText))) {
                    apps.add(search);
                }

                boardKanBan.setLstAppBase(apps);
            }

            items.add(boardKanBan);
        }

        return items;
    }

    public ArrayList<BoardKanBan> filter(ArrayList<BoardKanBan> kanBans, String fromday, String today, Integer[] status) {
        ArrayList<BoardKanBan> items = new ArrayList<>();

        for (BoardKanBan kanBan : kanBans) {
            BoardKanBan boardKanBan = new BoardKanBan();
            boardKanBan.setItemStepDefine(kanBan.getItemStepDefine());
            ArrayList<AppBase> apps = new ArrayList<>();

            for (AppBase search : kanBan.getLstAppBase()) {
                long l = Functions.share.formatStringToLongApi(search.getCreated());
                long l2 = Functions.share.formatStringToLong(fromday, "dd/MM/yyyy");
                long l3 = Functions.share.formatStringToLong(today, "dd/MM/yyyy");

                if (status.length > 0) {
                    if ((l >= l2 && l <= l3) && Arrays.stream(status).anyMatch(r -> r  == search.getStatusGroup())) {
                        apps.add(search);
                    }
                } else {
                    if (l >= l2 && l <= l3) {
                        apps.add(search);
                    }
                }

                boardKanBan.setLstAppBase(apps);
            }

            items.add(boardKanBan);
        }

        return items;
    }

    public void endDrag(BoardKanbanAdapter adapter, int originalPosition, int originalColumn, int newPosition, int newColumn) {
        ArrayList<BoardKanBan> boardKanBans = adapter.getListData();
        if (Math.abs(newColumn - originalColumn) >= 2) {
            listener.OnEndDragErr("", originalColumn);
            //listener.OnEndDragErr(Functions.share.getTitle("MESS_BOARD_AJACENTSTEP", "Chỉ được thao tác trên hai bước liền kề"), originalColumn);
        } else {
            // Drag qua phải
            if (newColumn > originalColumn) {
                AppBase app = boardKanBans.get(originalColumn).getLstAppBase().get(originalPosition);
                listener.OnEndDragSuccess(app, originalPosition, true);
            } else if (newColumn < originalColumn) {
                // Drag qua trái
                AppBase app = boardKanBans.get(originalColumn).getLstAppBase().get(originalPosition);
                listener.OnEndDragSuccess(app, originalPosition, false);
            }
        }
    }

    public void handleBoardAction(int originalPosition, AppBase base, boolean isNextAction) {
        String workflowId = Functions.share.getWorkflowItemIDByUrl(base.getItemUrl());
        WorkflowItem workflowItem = realm.where(WorkflowItem.class)
                .equalTo("ID", workflowId)
                .findFirst();

        Call<ApiObject<FormDetailInfo>> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).getTicketRequestControlDynamicForm(workflowId, String.valueOf(CurrentUser.getInstance().getUser().getLanguage()));
        call.enqueue(new Callback<ApiObject<FormDetailInfo>>() {
            @Override
            public void onResponse(Call<ApiObject<FormDetailInfo>> call, Response<ApiObject<FormDetailInfo>> response) {
                assert response.body() != null;
                FormDetailInfo formDetailInfo = response.body().getData();
                ViewRow action = formDetailInfo.getAction();
                ButtonAction buttonAction = null;
                for (ViewElement element : action.getElements()) {
                    if (isNextAction && (element.getID().equals(String.valueOf(Variable.WorkflowAction.Next))) || element.getID().equals(String.valueOf(Variable.WorkflowAction.Approve)) || element.getID().equals(String.valueOf(Variable.WorkflowAction.Submit))) {
                        buttonAction = new ButtonAction();
                        buttonAction.setID(Integer.parseInt(element.getID()));
                        buttonAction.setTitle(element.getTitle());
                        buttonAction.setValue(element.getValue());
                        buttonAction.setNotes(element.getNotes());
                        break;
                    } else if (!isNextAction && element.getID().equals(String.valueOf(Variable.WorkflowAction.Recall))) {
                        buttonAction = new ButtonAction();
                        buttonAction.setID(Integer.parseInt(element.getID()));
                        buttonAction.setTitle(element.getTitle());
                        buttonAction.setValue(element.getValue());
                        buttonAction.setNotes(element.getNotes());
                        break;
                    }
                }

                if (buttonAction == null) {
                    listener.OnEndDragErr(Functions.share.getTitle("MESS_BOARD_NOACTION", "Phiếu không có hành động tương ứng!"), originalPosition);
                    return;
                }

                // Next thì ko cần ý kiến -> gọi API luôn
                if (isNextAction) {
                    assert workflowItem != null;
                    submit(workflowItem, buttonAction, "");
                } else {
                    // back thì cần ý kiến
                    listener.OnBackAction(workflowItem, buttonAction);
                }
            }

            @Override
            public void onFailure(Call<ApiObject<FormDetailInfo>> call, Throwable t) {
                listener.OnSubmitErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
            }
        });
    }

    public void submit(WorkflowItem workflowItem, ButtonAction buttonAction, String comment) {
        HashMap<String, RequestBody> map = new HashMap<>();
        map.put("func", DetailFunc.share.toRequestBody(buttonAction.getValue()));
        map.put("fid", DetailFunc.share.toRequestBody(workflowItem.getID()));
        map.put("idea", DetailFunc.share.toRequestBody(comment));
        Call<Status> call = new ApiController().getApiRouteToken(BaseActivity.getToken()).sendControlDynamicAction(new MultipartBody.Part[0], map);
        call.enqueue(new Callback<Status>() {
            @Override
            public void onResponse(Call<Status> call, Response<Status> response) {
                if (response.isSuccessful()) {
                    assert response.body() != null;
                    if (response.body().getStatus().equals("SUCCESS")) {
                        listener.OnSubmitSuccess();
                    } else {
                        listener.OnSubmitErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                    }
                } else {
                    listener.OnSubmitErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
                }
            }

            @Override
            public void onFailure(Call<Status> call, Throwable t) {
                listener.OnSubmitErr(Functions.share.getTitle("TEXT_ACTIONFAIL", "Thao tác không thực hiện được. Xin vui lòng thử lại!"));
            }
        });
    }
}
