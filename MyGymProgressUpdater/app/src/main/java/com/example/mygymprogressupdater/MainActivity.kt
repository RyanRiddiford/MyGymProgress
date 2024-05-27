package com.example.mygymprogressupdater.ui.theme

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.view.View
import android.widget.*
import com.example.mygymprogressupdater.R
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.RequestBody.Companion.toRequestBody
import java.io.File
import java.io.IOException

class MainActivity : Activity() {
    private lateinit var urlSpinner: Spinner
    private lateinit var customUrlEditText: EditText
    private lateinit var uploadButton: Button
    private lateinit var responseMessageTextView: TextView

    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        urlSpinner = findViewById(R.id.url_spinner)
        customUrlEditText = findViewById(R.id.custom_url)
        uploadButton = findViewById(R.id.upload_button)
        responseMessageTextView = findViewById(R.id.response_message)

        ArrayAdapter.createFromResource(
            this,
            R.array.api_urls,
            android.R.layout.simple_spinner_item
        ).also { adapter ->
            adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
            urlSpinner.adapter = adapter
        }

        urlSpinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
                val selected = parent.getItemAtPosition(position).toString()
                if (selected == "Custom URL") {
                    customUrlEditText.visibility = View.VISIBLE
                } else {
                    customUrlEditText.visibility = View.GONE
                }
            }

            override fun onNothingSelected(parent: AdapterView<*>) {}
        }

        uploadButton.setOnClickListener {
            handleIntent(intent)
        }

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

        val selectedUrl = if (urlSpinner.selectedItem == "Custom URL") customUrlEditText.text.toString() else urlSpinner.selectedItem.toString()

        uploadCsvInChunks(file, selectedUrl)
    }

    private fun uploadCsvInChunks(file: File, url: String) {
        val chunkSize = 1024 * 1024 // 1MB
        val fileLength = file.length()
        val inputStream = file.inputStream()
        var bytesRead = 0L

        while (bytesRead < fileLength) {
            val buffer = ByteArray(chunkSize.toInt())
            val read = inputStream.read(buffer)
            if (read > 0) {
                val chunk = buffer.copyOf(read)
                uploadChunk(chunk, url)
                bytesRead += read
            }
        }
    }

    private fun uploadChunk(chunk: ByteArray, url: String) {
        val requestBody = chunk.toRequestBody("text/csv".toMediaTypeOrNull())

        val multipartBody = MultipartBody.Builder()
            .setType(MultipartBody.FORM)
            .addFormDataPart("file", "temp.csv", requestBody)
            .build()

        val request = Request.Builder()
            .url(url)
            .post(multipartBody)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                runOnUiThread {
                    Toast.makeText(this@MainActivity, "Failed to upload chunk", Toast.LENGTH_SHORT).show()
                    responseMessageTextView.text = "Error: ${e.message}"
                }
            }

            override fun onResponse(call: Call, response: Response) {
                runOnUiThread {
                    if (response.isSuccessful) {
                        Toast.makeText(this@MainActivity, "Chunk uploaded successfully!", Toast.LENGTH_SHORT).show()
                        responseMessageTextView.text = "Success: Chunk uploaded successfully!"
                    } else {
                        Toast.makeText(this@MainActivity, "Failed to upload chunk", Toast.LENGTH_SHORT).show()
                        responseMessageTextView.text = "Error: Failed to upload chunk"
                    }
                }
            }
        })
    }
}
