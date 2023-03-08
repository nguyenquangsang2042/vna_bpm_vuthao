package com.vuthao.bpmop.base.api;

import com.google.gson.JsonObject;
import com.vuthao.bpmop.base.model.ApiList;
import com.vuthao.bpmop.base.model.ApiObject;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppLanguage;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.Position;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.Status;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.DetailTask;
import com.vuthao.bpmop.base.model.custom.FormDetailInfo;
import com.vuthao.bpmop.base.model.custom.GridDetails;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.ObjectData;
import com.vuthao.bpmop.base.model.app.Settings;
import com.vuthao.bpmop.base.model.app.TimeLanguage;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.custom.ShareHistory;
import com.vuthao.bpmop.base.model.custom.WorkflowHistory;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Map;

import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Field;
import retrofit2.http.FieldMap;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.GET;
import retrofit2.http.Multipart;
import retrofit2.http.POST;
import retrofit2.http.Part;
import retrofit2.http.PartMap;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface Route {
    @POST("/_vti_bin/authentication.asmx")
    Call<ResponseBody> authencate(
            @Body RequestBody request);

    @POST("/_layouts/15/VuThao.BPMOP.API/ApiUser.ashx?func=login")
    @FormUrlEncoded
    Call<ApiObject<User>> Login(
            @Field("deviceInfo") String deviceInfo,
            @Field("loginType") String loginType);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&type=2&bname=BeanSettings")
    Call<ApiList<Settings>> getSettings(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiAppLang.ashx?func=get")
    Call<ApiList<AppLanguage>> getAppLanguage(
            @Query("lang") String langcode,
            @Query("Modified") String modified);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanNotify")
    Call<ApiList<Notify>> getNotify(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanAppBase")
    Call<ApiList<AppBase>> getAppBase(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanAppBase")
    Call<ApiList<AppBase>> getAppBaseItem(
            @Query("rid") String rid);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflow")
    Call<ApiList<Workflow>> getWorkflows(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanTimeLanguage")
    Call<ApiList<TimeLanguage>> getTimeLanguage(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanAppStatus")
    Call<ApiList<AppStatus>> getAppStatus(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanUser")
    Call<ApiList<User>> getUsers(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanGroup")
    Call<ApiList<Group>> getGroups(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowStepDefine")
    Call<ApiList<WorkflowStepDefine>> getWorkflowStepDefine(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanResourceView")
    Call<ApiList<ResourceView>> getResourceView(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowFollow")
    Call<ApiList<WorkflowFollow>> getWorkflowFollows(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @POST("/_layouts/15/VuThao.BPMOP.API/Filter.ashx?func=getList&resource=GetMyTaskV2")
    @FormUrlEncoded
    Call<ApiObject<AppBase>> getListFilterMyTask(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.API/Public.ashx?func=getList&resouce=GetFormField")
    @FormUrlEncoded
    Call<ApiObject<DetailList>> getDynamicFormField(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.MobileAPI/Filter.ashx?func=getList&resource=GetWorkFlowItem")
    @FormUrlEncoded
    Call<JsonObject> getDynamicWorkflowItem(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.MobileAPI/Filter.ashx?func=getList&resource=GetMyRequestV2")
    @FormUrlEncoded
    Call<ApiObject<AppBase>> getListFilterMyRequest(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.API/User.ashx?func=UpdateUser")
    @FormUrlEncoded
    Call<ObjectData> updateUserLanguage(
            @Field("data") String data);

    @GET("/_layouts/15/VuThao.BPMOP.API/AppLang.ashx?func=get")
    Call<ApiList<AppLanguage>> updateAppLanguage(
            @Query("lang") String lang);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowCategory")
    Call<ApiList<WorkflowCategory>> getWorkflowCategory(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowItem")
    Call<ApiList<WorkflowItem>> getWorkflowItems(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowStatus")
    Call<ApiList<WorkflowStatus>> getWorkflowStatus(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @GET("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=GetForm")
    Call<ApiObject<FormDetailInfo>> getTicketRequestControlDynamicForm(
            @Query("fid") String fid,
            @Query("lcid") String lcid);

    @GET("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=GetHistory")
    Call<ApiList<WorkflowHistory>> getListProcessHistory(
            @Query("fid") String workflowId);

    @Multipart
    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx")
    Call<Status> sendControlDynamicAction(
            @Part MultipartBody.Part[] files,
            @PartMap Map<String, RequestBody> params);

    @GET("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=GetShareHistory")
    Call<ApiList<ShareHistory>> getListShareHistory(
            @Query("fid") String workflowItemId);

    @POST("/_layouts/15/VuThao.BPMOP.API/Board.ashx?func=SetFavorite")
    @FormUrlEncoded
    Call<Status> setFavoriteWorkflow(
            @FieldMap Map<String, Object> data);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanPosition")
    Call<ApiList<Position>> getPositions(
            @Query("Modified") String modified,
            @Query("isFirst") String isFirst);

    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/OtherResouce.ashx?func=detail")
    @FormUrlEncoded
    Call<JsonObject> getDetailOtherResource(
            @Field("data") String data);

    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/ApiPublic.ashx?func=filter")
    @FormUrlEncoded
    Call<ApiList<Comment>> getListComment(
            @Field("filter") String data);

    @Multipart
    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/Comment.ashx?func=add")
    Call<Status> addComment(
            @Part MultipartBody.Part[] files,
            @Part("data") RequestBody data);

    @POST("/_layouts/15/VuThao.BPMOP.API/Like.ashx")
    @FormUrlEncoded
    Call<Status> addLikeComment(
            @Query("func") String func,
            @Field("data") String data);

    @GET("/_layouts/15/VuThao.BPMOP.API/Task.ashx?func=DetailTask")
    Call<ApiObject<DetailTask>> getDetailTaskForm(
            @Query("taskID") int taskId);

    @GET("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=BeanWorkflowItem")
    Call<ApiList<WorkflowItem>> getWorkflowItemById(
            @Query("rid") String rid);

    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/Task.ashx?func=CreateTask")
    @Multipart
    Call<Status> sendCreateTaskAction(
            @Part MultipartBody.Part[] files,
            @Part("data") RequestBody data);

    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/Task.ashx?func=DeleteTask")
    @FormUrlEncoded
    Call<Status> deleteDetailTaskForm(
            @Field("IID") String iid);

    @GET("/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=GetDataSource")
    Call<ApiList<LookupData>> getDataSource(
            @Query("LookupWebId") String lookupWebId,
            @Query("LookupList") String lookupList,
            @Query("LookupField") String lookupField);

    @GET("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=GetGridDataSource")
    Call<ApiObject<GridDetails>> getGridsDetails(
            @Query("fid") String workflowId);

    @POST("/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=excute&action=ReadNotify")
    @FormUrlEncoded
    Call<Status> setReadNotify(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx?func=getList&resource=ChartColumn")
    @FormUrlEncoded
    Call<ApiObject<DetailList>> getChartColumn(
            @Field("data") String data);

    @POST("/_layouts/15/VuThao.BPMOP.API/ApiResourceView.ashx?func=getList&resource=GetWorkFlowChart")
    @FormUrlEncoded
    Call<JsonObject> getWorkflowChart(
            @Field("data") String data);

    @POST("/workflow/_layouts/15/VuThao.BPMOP.API/TicketRequest.ashx?func=Filter")
    @FormUrlEncoded
    Call<ApiObject<AppBase>> getFilters(
            @Field("data") String data);
}