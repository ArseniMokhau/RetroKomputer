package com.aplication.retrokomputer_front.ui.BasicInstructions


import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding

class BasicInstructionsFragment : Fragment() {

    private var _binding: FragmentBasicBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter
    private lateinit var currentInformation : String

    private val instructionsList = listOf(
        "Load Immediate Values into Accumulator and Register X",
        "Save Accumulator to Memory",
        "Add Two Numbers",
        "Subtract Two Numbers",
        "Compare Two Values",
        "Branch If Not Equal",
        "Logical AND Operation",
        "Logical OR Operation"

    )

    private val opcodeList = listOf(
        "0xA9, 0x05, 0xA2, 0x03",
        "0xA9, 0x0A, 0x8D, 0x10",
        "0xA9, 0x05, 0x69, 0x03",
        "0xA9, 0x0A, 0xE9, 0x03",
        "0xA9, 0x0A, 0xC9, 0x0A",
        "0xA9, 0x05, 0xC9, 0x05, 0xB0, 0x03, 0x85, 0x00",
        "0xA9, 0x0F, 0x29, 0x05",
        "0xA9, 0x0A, 0x09, 0x05"
    )

    private val infoList = listOf(
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0xA2: Load the immediate value 0x03 into register X.",
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0x8D: Store the value in the accumulator into memory address 0x10.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x69: Add the value at the next memory location (0x03) to the accumulator.",
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0xE9: Subtract the value at the next memory location (0x03) from the accumulator.",
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0xC9: Compare the value at the next memory location (0x0A) with the accumulator.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0xC9: Compare the value at the next memory location (0x05) with the accumulator.\n0xB0, 0x03: Branch three bytes forward if the result of the comparison is not equal.\n0x85: Store contents of Accumulator in memory at 0x00.",
        "0xA9: Load the immediate value 0x0F into the accumulator.\n0x29: Perform a logical AND operation with the value at the next memory location (0x05).",
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0x09: Perform a logical OR operation with the value at the next memory location (0x05)."
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        Log.d("BasicInstructionsFragment", "onViewCreated")
        _binding = FragmentBasicBinding.inflate(inflater, container, false)
        val root = _binding?.root

        recyclerView = root?.findViewById(R.id.recyclerView) ?: RecyclerView(requireContext())
        adapter = InstructionAdapter(instructionsList) { position ->

            val selectedOpcode = opcodeList[position]
            currentInformation = infoList[position]
            val intent = Intent(requireContext(), InstructionDetailsActivity::class.java)
            intent.putExtra("OPCODE_KEY", selectedOpcode)
            intent.putExtra("Information", currentInformation)
            startActivity(intent)
        }

        recyclerView.adapter = adapter
        recyclerView.layoutManager = LinearLayoutManager(context)

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }




}
