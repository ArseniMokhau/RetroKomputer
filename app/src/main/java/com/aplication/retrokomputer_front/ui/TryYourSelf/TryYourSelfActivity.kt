package com.aplication.retrokomputer_front.ui.TryYourSelf

import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
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

class TryYourSelfActivity : AppCompatActivity() {

    private var resultMemory : String = "Empty"
    override fun onCreate(savenInstanceState: Bundle?){
        super.onCreate(savenInstanceState)
        setContentView(R.layout.activity_tryyourself)

        Log.d("TryYourSelfActivity", "onCreate called")

        val inputEditText : EditText = findViewById(R.id.InputEditText)
        val outputTextView : TextView = findViewById(R.id.outputTextView)
        val clearEverything : Button = findViewById(R.id.cleareverything)
        val dumpParametrs : Button = findViewById(R.id.dumpparametrs)
        val dumpButton : Button = findViewById(R.id.DumpButton)
        val startButton : Button = findViewById(R.id.singleexecute)
        var selectedOpcode : String





        startButton.setOnClickListener {
            try {


                // Create a JSON object
                selectedOpcode = inputEditText.text.toString()
                selectedOpcode = "\"$selectedOpcode\""

                Log.d("InstructionDetailsActivity", "Selected Opcode: $selectedOpcode")
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

                                if (result != null) {
                                    var otvet = result.split("//")
                                    resultMemory = """
                                            ${otvet[0]}
                                            Accumulator: ${otvet[1]}
                                            X: ${otvet[2]}
                                            Y: ${otvet[3]}
                                            Carry Flag: ${otvet[4]}
                                            Zero flag: ${otvet[5]}
                                            Negative flag: ${otvet[6]}
                                            Overflow flag: ${otvet[7]}
                                            Decimal flag: ${otvet[8]}
                                        """.trimIndent()

                                    outputTextView.text = "$resultMemory"
                                }} else {
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
                                outputTextView.text = "$result"
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

        dumpParametrs.setOnClickListener {
            try {
                // Execute a request to the server to dump memory using the dumpMemory endpoint
                val apiService = ApiClient.emulatorApiService
                val call = apiService.dumpParams()

                call.enqueue(object : Callback<ResponseBody> {
                    override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                        try {
                            if (response.isSuccessful) {
                                val result = response.body()?.string()
                                outputTextView.text = "Parametrs Dumped: $result"
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

        clearEverything.setOnClickListener {
            try {
                // Execute a request to the server to dump memory using the dumpMemory endpoint
                val apiService = ApiClient.emulatorApiService
                val call = apiService.clearEverything()

                call.enqueue(object : Callback<ResponseBody> {
                    override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                        try {
                            if (response.isSuccessful) {
                                val result = response.body()?.string()
                                outputTextView.text = "Parametrs Dumped: $result"
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



//    private fun handleSEPtemLogic(selectedOpcode: String) {
//        try {
//            // Place your specific logic for the SEF item here
//            // You can use the selectedOpcode along with your logic
//
//            // Create a RequestBody from the JSON string
//            val requestBody = RequestBody.create(MediaType.parse("application/json"), selectedOpcode)
//
//            // Perform the API call
//            val apiService = ApiClient.emulatorApiService
//            val call = apiService.singleExecuteProgram(requestBody)
//
//            call.enqueue(object : Callback<ResponseBody> {
//                override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
//                    try {
//                        if (response.isSuccessful) {
//                            val result = response.body()?.string()
//                            outputTextView.text = "SEF Operation Result: $result"
//                        } else {
//                            outputTextView.text = "Error: ${response.code()}, ${response.errorBody()?.string()}"
//                        }
//                    } catch (e: Exception) {
//                        outputTextView.text = "Error in onResponse: ${e.message}"
//                    }
//                }
//
//                override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
//                    outputTextView.text = "Failed to make API call: ${t.message}"
//                }
//            })
//
//        } catch (e: Exception) {
//            outputTextView.text = "Error in SEF item logic: ${e.message}"
//        }
//    }


}