package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.ui.API.ApiClient
import okhttp3.MediaType
import okhttp3.RequestBody
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class InstructionDetailsActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.operation_advanced)

        val outputTextView: TextView = findViewById(R.id.OuputWindowAdvanced)
        val startButton: Button = findViewById(R.id.StartButtonAdvanced)
        val dumpButton: Button = findViewById(R.id.DumpButtonAdvanced)

        var selectedOpcode = intent.getStringExtra("OPCODE_KEY") ?: ""

        outputTextView.text = "Selected Opcode: $selectedOpcode"

        // ...

        // ...

        startButton.setOnClickListener {
            try {
                Log.d("InstructionDetailsActivity", "Selected Opcode: $selectedOpcode")

                // Create a JSON object
                selectedOpcode = "\"$selectedOpcode\""

                // Create a RequestBody from the JSON string
                val requestBody = RequestBody.create(MediaType.parse("application/json"), selectedOpcode)

                // Perform the API call
                val apiService = ApiClient.emulatorApiService
                val call = apiService.executeProgram(requestBody)

                call.enqueue(object : Callback<ResponseBody> {
                    override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                        try {
                            if (response.isSuccessful) {
                                val result = response.body()?.string()
                                outputTextView.text = "Result: $result"
                            } else {
                                outputTextView.text = "Error: ${response.code()}, ${response.errorBody()?.string()}"
                            }
                        } catch (e: Exception) {
                            outputTextView.text = "Error in onResponse: ${e.message}"
                        }
                    }

                    override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                        outputTextView.text = "Failed to make API call: ${t.message}"
                    }
                })

            } catch (e: Exception) {
                outputTextView.text = "Error in startButton onClick: ${e.message}"
            }
        }





        dumpButton.setOnClickListener {
            try {
                // Execute a request to the server to dump memory using the dumpMemory endpoint
                val apiService = ApiClient.emulatorApiService
                val call = apiService.dumpMemory()

                call.enqueue(object : Callback<ResponseBody> {
                    override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                        try {
                            if (response.isSuccessful) {
                                val result = response.body()?.string()
                                outputTextView.text = "Memory Dumped: $result"
                            } else {
                                outputTextView.text = "Error: ${response.code()}, ${response.errorBody()?.string()}"
                            }
                        } catch (e: Exception) {
                            outputTextView.text = "Error in onResponse: ${e.message}"
                        }
                    }

                    override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                        outputTextView.text = "Failed to make API call: ${t.message}"
                    }
                })

            } catch (e: Exception) {
                outputTextView.text = "Error in dumpButton onClick: ${e.message}"
            }
        }
    }
}