using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using CoffeeTea.Models;

namespace CoffeeTea.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly string _maNhanVien;

        private string _fullName;
        private string _username;
        private string _roleName;
        private string _phoneNumber;
        private string _email;
        private string _address;

        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;

        private string _successMessage;
        private string _errorMessage;

        public ProfileViewModel(NhanVien authenticatedUser)
        {
            _maNhanVien = authenticatedUser != null ? authenticatedUser.MaNhanVien : null;

            SaveProfileCommand = new RelayCommand(_ => ExecuteSaveProfile(), _ => CanEditProfile);
            ChangePasswordCommand = new RelayCommand(_ => ExecuteChangePassword(), _ => CanEditProfile);

            LoadProfileData();
        }

        public event Action PasswordInputsClearRequested;

        public ICommand SaveProfileCommand { get; private set; }

        public ICommand ChangePasswordCommand { get; private set; }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName == value)
                {
                    return;
                }

                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                {
                    return;
                }

                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string RoleName
        {
            get { return _roleName; }
            set
            {
                if (_roleName == value)
                {
                    return;
                }

                _roleName = value;
                OnPropertyChanged(nameof(RoleName));
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber == value)
                {
                    return;
                }

                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                {
                    return;
                }

                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                if (_address == value)
                {
                    return;
                }

                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string CurrentPassword
        {
            get { return _currentPassword; }
            set
            {
                if (_currentPassword == value)
                {
                    return;
                }

                _currentPassword = value;
                OnPropertyChanged(nameof(CurrentPassword));
            }
        }

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (_newPassword == value)
                {
                    return;
                }

                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword == value)
                {
                    return;
                }

                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                if (_successMessage == value)
                {
                    return;
                }

                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool CanEditProfile
        {
            get { return !string.IsNullOrWhiteSpace(_maNhanVien); }
        }

        private void LoadProfileData()
        {
            ClearMessages();

            if (!CanEditProfile)
            {
                ErrorMessage = "Không xác định được tài khoản đang đăng nhập.";
                return;
            }

            try
            {
                using (QL_CoffeeTeaEntities context = new QL_CoffeeTeaEntities())
                {
                    NhanVien user = context.NhanViens
                        .Include(nv => nv.VaiTro)
                        .FirstOrDefault(nv => nv.MaNhanVien == _maNhanVien);

                    if (user == null)
                    {
                        ErrorMessage = "Không tìm thấy thông tin tài khoản trong cơ sở dữ liệu.";
                        return;
                    }

                    FullName = user.HoTen;
                    Username = user.TenDangNhap;
                    PhoneNumber = user.SoDienThoai;
                    Email = user.Email;
                    Address = user.DiaChi;
                    RoleName = user.VaiTro != null ? user.VaiTro.TenVaiTro : "Không xác định";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể tải thông tin tài khoản. Vui lòng kiểm tra kết nối dữ liệu.";
            }
        }

        private void ExecuteSaveProfile()
        {
            ClearMessages();

            if (!CanEditProfile)
            {
                ErrorMessage = "Không xác định được tài khoản để cập nhật.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Họ tên không được để trống.";
                return;
            }

            try
            {
                using (QL_CoffeeTeaEntities context = new QL_CoffeeTeaEntities())
                {
                    NhanVien user = context.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == _maNhanVien);
                    if (user == null)
                    {
                        ErrorMessage = "Tài khoản không tồn tại hoặc đã bị xóa.";
                        return;
                    }

                    user.HoTen = FullName.Trim();
                    user.SoDienThoai = NormalizeNullable(PhoneNumber);
                    user.Email = NormalizeNullable(Email);
                    user.DiaChi = NormalizeNullable(Address);

                    context.SaveChanges();
                }

                SuccessMessage = "Cập nhật hồ sơ thành công.";
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể cập nhật hồ sơ. Vui lòng thử lại.";
            }
        }

        private void ExecuteChangePassword()
        {
            ClearMessages();

            if (!CanEditProfile)
            {
                ErrorMessage = "Không xác định được tài khoản để đổi mật khẩu.";
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ mật khẩu hiện tại, mật khẩu mới và xác nhận mật khẩu.";
                return;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return;
            }

            if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
            {
                ErrorMessage = "Xác nhận mật khẩu không khớp.";
                return;
            }

            if (string.Equals(CurrentPassword, NewPassword, StringComparison.Ordinal))
            {
                ErrorMessage = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                return;
            }

            try
            {
                using (QL_CoffeeTeaEntities context = new QL_CoffeeTeaEntities())
                {
                    NhanVien user = context.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == _maNhanVien);
                    if (user == null)
                    {
                        ErrorMessage = "Tài khoản không tồn tại hoặc đã bị xóa.";
                        return;
                    }

                    if (!string.Equals(user.MatKhau, CurrentPassword, StringComparison.Ordinal))
                    {
                        ErrorMessage = "Mật khẩu hiện tại không chính xác.";
                        return;
                    }

                    user.MatKhau = NewPassword;
                    context.SaveChanges();
                }

                SuccessMessage = "Đổi mật khẩu thành công.";
                ClearPasswordInputs();
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể đổi mật khẩu. Vui lòng thử lại.";
            }
        }

        private void ClearMessages()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void ClearPasswordInputs()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            PasswordInputsClearRequested?.Invoke();
        }

        private static string NormalizeNullable(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
