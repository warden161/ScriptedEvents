﻿using Exiled.API.Features;
using ScriptedEvents.Conditions;
using ScriptedEvents.Conditions.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.API.Helpers
{
    public static class ConditionHelper
    {
        public static ReadOnlyCollection<IBooleanCondition> BooleanConditions { get; } = new List<IBooleanCondition>()
        {
            new GreaterThan(),
            new LessThan(),
            new Equal(),
        }.AsReadOnly();

        // StackOverflow my beloved
        public static double Math(string expression)
        {
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (double)loDataTable.Rows[0]["Eval"];
        }

        public static ConditionResponse Evaluate(string input)
        {
            input = ConditionVariables.ReplaceVariables(input.Replace(" ", "")).Trim(); // Kill all whitespace & replace variables

            // Code for simple checks
            if (input.ToLowerInvariant() is "true")
                return new(true, true, string.Empty);

            if (input.ToLowerInvariant() is "false")
                return new(true, false, string.Empty);

            // Code for conditions with boolean operator
            IBooleanCondition condition = null;
            foreach (IBooleanCondition con in BooleanConditions)
            {
                if (input.Contains(con.Symbol))
                {
                    condition = con;
                }
            }
            if (condition is null)
                return new(false, false, $"Invalid condition operator provided! Condition: '{input}'");

            string[] split = input.Split(condition.Symbol);

            if (split.Length != 2)
                return new(false, false, $"Malformed condition provided! Condition: '{input}'");

            double left;
            try
            {
                left = Math(split[0]);
            }
            catch (Exception ex)
            {
                return new(false, false, $"Provided expression on the lefthand side is invalid. Lefthand: '{split[0]}' Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            double right;
            try
            {
                right = Math(split[1]);
            }
            catch (Exception ex)
            {
                return new(false, false, $"Provided expression on the righthand side is invalid. Righthand: '{split[1]}' Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            return new(true, condition.Execute((float)left, (float)right), string.Empty);
        }
    }

    public class ConditionResponse
    {
        public bool Success { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }

        public ConditionResponse(bool success, bool passed, string message)
        {
            Success = success;
            Passed = passed;
            Message = message;
        }

        public override string ToString()
        {
            return $"SUCCESS: {Success} | PASSED: {Passed} | MESSAGE: {(Message ?? "N/A")}";
        }
    }
}
