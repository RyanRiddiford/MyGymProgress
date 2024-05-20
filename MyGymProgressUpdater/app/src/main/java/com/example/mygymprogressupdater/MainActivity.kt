package com.example.mygymprogressupdater.ui.theme

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import okhttp3.*
import java.io.File
import java.io.IOException

class MainActivity : Activity() {
    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        handleIntent(intent)
    }

    private fun handleIntent(intent: Intent?) {
        intent?.let {
            if (it.action == Intent.ACTION_SEND && it.type == "text/csv") {
                (it.getParcelableExtra<Uri>(Intent.EXTRA_STREAM))?.let { uri ->
                    sendCsvToBackend(uri)
                }
            }
        }
    }

    private fun sendCsvToBackend(uri: Uri) {
        val inputStream = contentResolver.openInputStream(uri)
        val file = File(cacheDir, "temp.csv")
        file.outputStream().use { fileOut ->
            inputStream?.copyTo(fileOut)
        }

        val requestBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("file", "temp.csv",
                file.asRequestBody("text/csv".toMediaTypeOrNull()))
            .build()

        val request = Request.Builder()
            .url("http://localhost:5123/api/sendTrainingSessions")
            .post(requestBody)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                e.printStackTrace()
                // Optionally handle failure here
            }

            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    println("File uploaded successfully!")
                } else {
                    println("Failed to upload file")
                }
            }
        })
    }
}
