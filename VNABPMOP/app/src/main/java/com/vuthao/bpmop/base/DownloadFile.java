package com.vuthao.bpmop.base;

import android.app.Activity;
import android.os.AsyncTask;
import android.util.Log;

import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.UUID;

import io.realm.Realm;

@SuppressWarnings("ALL")
public class DownloadFile extends AsyncTask<String, String, String> {
    private String userId = CurrentUser.getInstance().getUser().getID();
    @Override
    protected void onPreExecute() {
        super.onPreExecute();
        BaseActivity.sBaseActivity.showProgressDialog();
    }

    @Override
    protected String doInBackground(String... f_url) {
        int count;
        InputStream input = null;
        OutputStream output = null;
        HttpURLConnection connection = null;
        String path = "";
        try {
            String fileName = f_url[0].split(";#")[1];;
            File dirs = new File(BaseActivity.sBaseActivity.getFilesDir().toString(), userId);
            File f = new File(dirs, fileName);
            path = f.toString();

            if (!dirs.exists()) {
                dirs.mkdirs();
            }

            if (f.exists()) {
                return path;
            }

            URL url = new URL(f_url[0].split(";#")[0]);
            connection = (HttpURLConnection) url.openConnection();
            connection.addRequestProperty("Cookie", BaseActivity.getToken());
            connection.addRequestProperty("ACCEPT", "*/*");
            connection.connect();

            if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
                return "Server returned HTTP " + connection.getResponseCode()
                        + " " + connection.getResponseMessage();
            }

            int lenghtOfFile = connection.getContentLength();

            input = new BufferedInputStream(connection.getInputStream());

            output = new FileOutputStream(path);

            byte[] data = new byte[1024];

            long total = 0;

            while ((count = input.read(data)) != -1) {
                total += count;
                publishProgress("" + (int) ((total * 100) / lenghtOfFile));

                output.write(data, 0, count);
            }

            output.flush();
            output.close();
            input.close();
        } catch (IOException ex) {
            Log.d("ERR DownloadFile", ex.getMessage());
        } finally {
            try {
                if (output != null)
                    output.close();
                if (input != null)
                    input.close();
            } catch (IOException ignored) {
            }

            if (connection != null)
                connection.disconnect();
        }

        return path;
    }

    @Override
    protected void onPostExecute(String path) {
        BaseActivity.sBaseActivity.hideProgressDialog();
        DetailFunc.share.openFile(BaseActivity.sBaseActivity, path);
    }
}
