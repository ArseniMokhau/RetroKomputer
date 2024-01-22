package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R

class InstructionAdapter(
    private val instructions: List<String>,
    private val onItemClick: (Int) -> Unit
) : RecyclerView.Adapter<InstructionAdapter.ViewHolder>() {

    class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val textView: TextView = view.findViewById(R.id.text_AdvancedInstructions)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_instruction_advanced, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.textView.text = instructions[position]
        holder.itemView.setOnClickListener { onItemClick(position) }
    }

    override fun getItemCount(): Int = instructions.size
}