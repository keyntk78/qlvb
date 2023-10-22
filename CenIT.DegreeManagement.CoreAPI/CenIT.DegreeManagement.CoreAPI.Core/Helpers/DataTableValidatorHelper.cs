using System.Data;


namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public static class DataTableValidatorHelper
    {
        public static ValidationResult ValidateDataTable(DataTable dataTable, List<ValidationRule> validationRules)
        {
            Dictionary<string, HashSet<string>> uniqueValues = new Dictionary<string, HashSet<string>>();
            ValidationResult validationResult = new ValidationResult(1);

            for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                DataRow row = dataTable.Rows[rowIndex];
                bool hasError = false;

                foreach (var rule in validationRules)
                {
                    string columnName = rule.ColumnName;
                    string value = row[columnName].ToString();

                    if (rule.IsRequired && string.IsNullOrEmpty(value))
                    {
                        validationResult = new ValidationResult(-1, $"Dòng {rowIndex + 2}: Trường {rule.ColumnName} không được để trống.");
                        hasError = true;
                        break;
                    }

                    if (rule.IsUnique)
                    {
                        if (!uniqueValues.TryGetValue(columnName, out var uniqueSet))
                        {
                            uniqueSet = new HashSet<string>();
                            uniqueValues[columnName] = uniqueSet;
                        }

                        if (uniqueSet.Contains(value))
                        {
                            validationResult = new ValidationResult(-1, $"Dòng {rowIndex + 2}: Trường {rule.ColumnName} bị trùng lặp.");
                            hasError = true;
                            break;
                        }
                        else
                        {
                            uniqueSet.Add(value);
                        }
                    }

                    if (rule.CustomValidator != null && !rule.CustomValidator(value))
                    {
                        validationResult = new ValidationResult(-1, $"Dòng {rowIndex + 2}: Trường {rule.ColumnName} không hợp lệ.");
                        hasError = true;
                        break;
                    }
                }

                if (hasError)
                {
                    break; // Nếu có lỗi, dừng vòng lặp và trả về kết quả
                }
            }

            return validationResult;
        }

        public class ValidationRule
        {
            public string ColumnName { get; set; }
            public bool IsRequired { get; set; }
            public bool IsUnique { get; set; }
            public Func<string, bool> CustomValidator { get; set; }
        }

        public class ValidationResult
        {
            public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }

            public ValidationResult(int errorCode, string errorMessage = "")
            {
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }
        }

    }
}
