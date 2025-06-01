using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public int CurrentStep { get; set; } = 0;

        public bool IsApproved { get; set; } = false;

        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();

        /// <summary>
        /// Adds a step to the workflow.
        /// </summary>
        /// <param name="Step">The step to add</param>
        public void AddStep(WorkflowStep Step)
        {
            this.Steps.Add(Step);
        }


        /// <summary>
        /// Gets the current step in the workflow.
        /// </summary>
        /// <returns>The current step</returns>
        public WorkflowStep GetCurrentStep()
        {
            return this.Steps.OrderBy(s=> s.Step??int.MaxValue).ToList()[this.CurrentStep];
        }

        /// <summary>
        /// Approves the current step in the workflow. If the current step is the
        /// last one, it will set the isApproved flag to true.
        /// </summary>
        public void Approve()
        {
            if (Steps.Count > 0 && CurrentStep >= 0 && CurrentStep < Steps.Count - 1)
            {
                CurrentStep++;
            }
            else
            {
                IsApproved = true;
            }
        }

        public void Approve(string reason)  
        {
            // Guarda a razão no passo atual antes de avançar
            if (Steps.Count > 0 && CurrentStep >= 0 && CurrentStep < Steps.Count)
            {
                Steps[CurrentStep].Reason = reason;
            }

            if (CurrentStep < Steps.Count - 1)
            {
                CurrentStep++;
            }
            else
            {
                IsApproved = true;
            }
        }


        /// <summary>
        /// Denies the current step in the workflow and resets the step to the first one.
        /// </summary>
        public void Deny()
        {
            this.CurrentStep = 0;
            this.IsApproved = false;
        }


        /// <summary>
        /// Denies the current step in the workflow, resets the step to the first one and
        /// sets the reason for the denial.
        /// </summary>
        /// <param name="Reason">The reason for the denial</param>
        public void Deny(string Reason)
        {
            this.CurrentStep = 0;
            this.IsApproved = false;
            this.GetCurrentStep().Reason = Reason;
        }

        /// <summary>
        /// Gets the current permission required for the current step in the workflow
        /// </summary>
        /// <returns>The current permission</returns>
        public string GetCurrentPermission()
        {
            return this.GetCurrentStep().Permission;
        }
    }
}