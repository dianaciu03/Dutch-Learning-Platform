﻿using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface IAccountManager
    {
        bool CreateTeacherAccount(CreateTeacherAccountRequest request);
        bool CreateStudentAccount(CreateStudentAccountRequest request);
        AccountResponse GetAllAccounts();
        AccountResponse GetTeacherAccountById(string id);
        AccountResponse GetStudentAccountById(string id);
        bool DeleteAccount(string id);

    }
}
