package com.aplication.retrokomputer_front.ui.TryYourSelf

import android.os.Bundle
import android.util.Log
import android.view.MenuItem
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.PopupMenu
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



    override fun onCreate(savenInstanceState: Bundle?){
        super.onCreate(savenInstanceState)
        setContentView(R.layout.activity_tryyourself)

        Log.d("TryYourSelfActivity", "onCreate called")

        val inputEditText : EditText = findViewById(R.id.InputEditText)
        val outputTextView : TextView = findViewById(R.id.outputTextView)
        val dumpButton : Button = findViewById(R.id.DumpButton)
        val advancebutton : Button = findViewById(R.id.advancedoptionsbutton)
        val startButton : Button = findViewById(R.id.StartButton)
        val infoButton : Button = findViewById(R.id.InformationButton)
        var selectedOpcode : String

        advancebutton.setOnClickListener {
            showPopupMenu(advancebutton)
        }


        startButton.setOnClickListener {
            try {


                // Create a JSON object
                selectedOpcode = inputEditText.text.toString()

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

    private fun showPopupMenu(view: View) {
        val popupMenu = PopupMenu(this, view)
        popupMenu.menuInflater.inflate(R.menu.popup_menu, popupMenu.menu)

        popupMenu.setOnMenuItemClickListener { item: MenuItem ->
            when (item.itemId) {
                R.id.SEP -> {
                    // Handle the action for menu item 1
                    true
                }
                R.id.DP -> {
                    // Handle the action for menu item 2
                    true
                }
                R.id.CP -> {
                    // Handle the action for menu item 3
                    true
                }
                R.id.CE -> {
                    // Handle the action for menu item 4
                    true
                }
                else -> false
            }
        }

        popupMenu.show()
    }




}