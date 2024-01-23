package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentAdvancedBinding
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionAdapter
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionDetailsActivity

class AdvancedInstructionsFragment : Fragment() {

    private var _binding: FragmentAdvancedBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter
    private lateinit var currentInformation : String

    private val instructionsList = listOf(
        "Logical XOR Operation",
        "Right Shift Operation",
        "Left Shift Operation",
        "Increment and Decrement Operations",
        "Load Value from Memory into Accumulator",
        "Rotate Left Operation",
        "Rotate Right Operation",
        "Jump If Carry Flag is Cleared",
        "Load and Store Index Register Y"
    )

    private val opcodeList = listOf(
        "0xA9, 0x0A, 0x49, 0x05",
        "0xA9, 0x0A, 0x6A",
        "0xA9, 0x05, 0x0A",
        "0xA9, 0x05, 0xA2, 0x03, 0xE6, 0x10, 0xE8, 0xE8, 0xE8",
        "0xA9, 0x05, 0x8D, 0x10, 0xAD, 0x10",
        "0xA9, 0x05, 0x2A",
        "0xA9, 0x05, 0x6A",
        "0xA9, 0x05, 0x18, 0x90, 0x03, 0xEA, 0x85, 0x00",
        "0xA0, 0x05, 0xA2, 0x03, 0x99, 0x10"
    )

    private val infoList = listOf(
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0x49: Perform a logical exclusive OR operation with the value at the next memory location (0x05).",
        "0xA9: Load the immediate value 0x0A into the accumulator.\n0x6A: Shift the bits in the accumulator one position to the right.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x0A: Shift the bits in the accumulator one position to the left.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0xA2: Load the immediate value 0x03 into XRegister.\n0xE6: Increment the value at memory address 0x10.\n0xE8: Increment the value in XRegister.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x8D: Store the value in the accumulator into memory address 0x10.\n0xAD: Load the value from memory address 0x10 into the accumulator.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x2A: Rotate the bits in the accumulator one position to the left through the carry flag.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x6A: Rotate the bits in the accumulator one position to the right through the carry flag.",
        "0xA9: Load the immediate value 0x05 into the accumulator.\n0x18: Clear the carry flag.\n0x90: Branch three bytes forward if the carry flag is clear.\n0x85: Store contents of Accumulator in memory at 0x00.",
        "0xA0: Load the immediate value 0x05 into register Y.\n0xA2: Load the immediate value 0x03 into register X.\n0x99: Store the value in register Y into memory using register X as an index."

    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentAdvancedBinding.inflate(inflater, container, false)
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