package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.TextView
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.ui.API.ApiClient
import com.google.android.material.bottomnavigation.BottomNavigationView
import okhttp3.MediaType
import okhttp3.RequestBody
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class InstructionDetailsActivity : AppCompatActivity() {

    private lateinit var outputTextView: TextView
    private lateinit var resultTextView : TextView
    private lateinit var startButton: Button
    private var resultMemory : String = "Empty"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.operation_advanced)

        outputTextView = findViewById(R.id.outputTextViewAdvanced)
        resultTextView = findViewById(R.id.ResultTextViewAdvanced)
        startButton = findViewById(R.id.startButtonAdvanced)


        var selectedOpcode = intent.getStringExtra("OPCODE_KEY") ?: ""

        outputTextView.text = "Selected Opcode: \n $selectedOpcode"

        val bottomNavigationView: BottomNavigationView = findViewById(R.id.bottomNavigationView)

        bottomNavigationView.setOnNavigationItemSelectedListener { menuItem ->
            when (menuItem.itemId) {
                R.id.menu_information -> {
                    // Обработка нажатия на "Information"
                    showInformationDialog()
                    true
                }
                R.id.menu_memory -> {
                    // Обработка нажатия на "Dump Memory"
                    showMemoryDialog()

                    true
                }
                else -> false
            }
        }

        startButton.setOnClickListener {
            singleexecuteProgram(selectedOpcode)
        }
    }




    private fun singleexecuteProgram(selectedOpcode: String) {
        Log.d("InstructionDetailsActivity", "Selected Opcode: $selectedOpcode")

        // Create a JSON object
        val requestBody = RequestBody.create(MediaType.parse("application/json"), "\"$selectedOpcode\"")

        // Perform the API call
        val apiService = ApiClient.emulatorApiService
        val call = apiService.singleExecuteProgram(requestBody)

        call.enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                try {
                    if (response.isSuccessful) {
                        val result = response.body()?.string()
                        resultTextView.text = "Result: Success"
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

                        }
                    } else {
                        resultTextView.text = "Error: ${response.code()}, ${response.errorBody()?.string()}"
                    }
                } catch (e: Exception) {
                    resultTextView.text = "Error in onResponse: ${e.message}"
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                resultTextView.text = "Failed to make API call: ${t.message}"
            }
        })
    }



    private fun showInformationDialog() {
        val informationMessage = ""

        val builder = AlertDialog.Builder(this)
        builder.setTitle("Information")
            .setMessage(informationMessage)
            .setPositiveButton("OK") { dialog, _ ->
                dialog.dismiss()
            }
            .show()
    }

    private fun showMemoryDialog() {

        val informationMessage = resultMemory

        val builder = AlertDialog.Builder(this)
        builder.setTitle("Memory")
            .setMessage(informationMessage)
            .setPositiveButton("OK") { dialog, _ ->
                dialog.dismiss()
            }
            .show()
    }
}
