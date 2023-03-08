package com.vuthao.bpmop.base.api;

import java.util.concurrent.TimeUnit;
import android.text.TextUtils;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.activity.BaseActivity;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class ApiController {

    Route getApiRoute() {
        if (!TextUtils.isEmpty("")) {
            return getApiRouteToken();
        }
        return getApiRoute("");
    }

    Route getApiRouteToken() {
        return getApiRouteToken("", BaseActivity.getToken());
    }

    public Route getApiRouteToken(final String token) {
        return getApiRouteToken("", token);
    }

    Route getApiRoute(String url) {
        if (url.contains(Constants.BASE_URL)) {
            url = url.replaceAll(Constants.BASE_URL, "");
        }

        url = Constants.BASE_URL + url;

        if (!TextUtils.isEmpty("")) {
            return getApiRouteToken(url, "");
        }

        Gson gson = new GsonBuilder()
                .setLenient()
                .create();

        Retrofit.Builder builder = new Retrofit.Builder()
                .baseUrl(url)
                .addConverterFactory(GsonConverterFactory.create(gson));

        Retrofit retrofit = builder.build();
        return retrofit.create(Route.class);
    }

    Route getApiRouteToken(String url, final String token) {
        if (url.contains(Constants.BASE_URL)) {
            url = url.replaceAll(Constants.BASE_URL, "");
        }
        url = Constants.BASE_URL + url;
        OkHttpClient.Builder httpClient = new OkHttpClient.Builder();

        httpClient.connectTimeout(5, TimeUnit.MINUTES)
                .writeTimeout(5, TimeUnit.MINUTES)
                .readTimeout(5, TimeUnit.MINUTES);

        httpClient.addInterceptor(chain -> {
            Request original = chain.request();

            Request request = original.newBuilder()
                    .addHeader("Cookie", token)
                    .addHeader("Content-Type", "application/json")
                    .addHeader("Accept","application/json")
                    .addHeader("Accept", "application/x-www-form-urlencoded")
                    .method(original.method(), original.body())
                    .build();

            return chain.proceed(request);
        });

        HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
        logging.setLevel(HttpLoggingInterceptor.Level.BODY);
        httpClient.addInterceptor(logging);

        Gson gson = new GsonBuilder()
                .setLenient()
                .create();

        Retrofit.Builder builder = new Retrofit.Builder()
                .baseUrl(url)
                .addConverterFactory(GsonConverterFactory.create(gson))
                .client(httpClient.build());
        Retrofit retrofit = builder.build();
        return retrofit.create(Route.class);
    }

    public interface StatusListener {
        void OnSuccess();

        void OnError(String error);
    }
}

