using static CenIT.DegreeManagement.CoreAPI.Core.Helpers.DataTableValidatorHelper;

namespace CenIT.DegreeManagement.CoreAPI.Validation
{
    public static class HocSinhValidationRules
    {
        public static List<ValidationRule> GetRules(string[] monThis, string[] danTocs)
        {
            return new List<ValidationRule>
            {
                new ValidationRule
                {
                    ColumnName = "STT",
                    IsRequired = true,
                    IsUnique = true
                },
                new ValidationRule
                {
                    ColumnName = "Hoten",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "CCCD",
                    IsRequired = true,
                    IsUnique = true
                },
                new ValidationRule
                {
                    ColumnName = "GioiTinh",
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == "nam" || value.ToLower() == "nữ"
                },
                new ValidationRule
                {
                    ColumnName = "NoiSinh",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "NgaySinh",
                    IsRequired = true,
                    CustomValidator = value => DateTime.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "DanToc",
                    IsRequired = true,
                    CustomValidator = value => danTocs.Contains(value)
                },
                  new ValidationRule
                {
                    ColumnName = "Mon1",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                    new ValidationRule
                {
                    ColumnName = "Mon2",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                      new ValidationRule
                {
                    ColumnName = "Mon3",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                        new ValidationRule
                {
                    ColumnName = "Mon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                          new ValidationRule
                {
                    ColumnName = "Mon5",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                            new ValidationRule
                {
                    ColumnName = "Mon6",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon2",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon3",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon5",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon6",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemXTN",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemKK",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemTB12",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "XepLoai",
                    IsRequired = true,
                    CustomValidator = value =>  value.ToLower() == "khá" || value.ToLower() == "giỏi" || value.ToLower() == "trung bình" || value.ToLower() == "yếu"
                },
                 new ValidationRule
                {
                    ColumnName = "DiaChi",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "Lop",
                    IsRequired = true
                },
            };
        }
    }
}
