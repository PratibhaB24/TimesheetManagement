-- =====================================================
-- Timesheet Management System - SQL Seed Data Script
-- =====================================================
-- This script seeds the database with initial test data
-- Run this after applying the EF Core migrations

USE TimesheetManagementDb;
GO

-- Clear existing data (in correct order due to foreign keys)
DELETE FROM TimesheetEntries;
DELETE FROM Timesheets;
DELETE FROM ProjectAssignments;
DELETE FROM Projects;
DELETE FROM Users;
GO

-- Reset Identity columns
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Projects', RESEED, 0);
DBCC CHECKIDENT ('ProjectAssignments', RESEED, 0);
DBCC CHECKIDENT ('Timesheets', RESEED, 0);
DBCC CHECKIDENT ('TimesheetEntries', RESEED, 0);
GO

-- =====================================================
-- SEED USERS
-- =====================================================
-- Password for all users: "Password123" (hashed using SHA256)
-- In production, use proper password hashing (BCrypt/Argon2)

INSERT INTO Users (FullName, Email, PasswordHash, Role, IsActive, CreatedOn)
VALUES
    -- Managers (Role = 2)
    ('John Smith', 'john.smith@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 2, 1, GETUTCDATE()),
    ('Sarah Johnson', 'sarah.johnson@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 2, 1, GETUTCDATE()),
    
    -- Employees (Role = 1)
    ('Mike Wilson', 'mike.wilson@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 1, 1, GETUTCDATE()),
    ('Emily Brown', 'emily.brown@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 1, 1, GETUTCDATE()),
    ('David Lee', 'david.lee@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 1, 1, GETUTCDATE()),
    ('Lisa Anderson', 'lisa.anderson@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 1, 1, GETUTCDATE()),
    ('James Taylor', 'james.taylor@company.com', 'scrypt:32768:8:1$h0qh/Mz7YWK1/3AX$21a1f1bf7c7bc4b99a8c9d0e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0', 1, 0, GETUTCDATE()); -- Inactive employee
GO

-- =====================================================
-- SEED PROJECTS
-- =====================================================
-- Status: 1 = Active, 2 = Inactive

INSERT INTO Projects (Code, Name, ClientName, IsBillable, Status, CreatedOn)
VALUES
    ('PRJ001', 'E-Commerce Platform', 'Retail Corp', 1, 1, GETUTCDATE()),
    ('PRJ002', 'Mobile Banking App', 'First National Bank', 1, 1, GETUTCDATE()),
    ('PRJ003', 'HR Management System', 'Internal', 0, 1, GETUTCDATE()),
    ('PRJ004', 'Data Analytics Dashboard', 'Analytics Inc', 1, 1, GETUTCDATE()),
    ('PRJ005', 'Legacy System Migration', 'Old Systems Ltd', 1, 2, GETUTCDATE()), -- Inactive project
    ('PRJ006', 'Customer Portal', 'Service Pro', 1, 1, GETUTCDATE()),
    ('INT001', 'Training & Development', 'Internal', 0, 1, GETUTCDATE()),
    ('INT002', 'Administrative Tasks', 'Internal', 0, 1, GETUTCDATE());
GO

-- =====================================================
-- SEED PROJECT ASSIGNMENTS
-- =====================================================

INSERT INTO ProjectAssignments (UserId, ProjectId, StartDate, EndDate, CreatedOn)
VALUES
    -- Mike Wilson (UserId: 3) assignments
    (3, 1, '2024-01-01', NULL, GETUTCDATE()),      -- E-Commerce Platform (ongoing)
    (3, 3, '2024-01-01', NULL, GETUTCDATE()),      -- HR Management System (ongoing)
    (3, 7, '2024-01-01', NULL, GETUTCDATE()),      -- Training & Development (ongoing)
    
    -- Emily Brown (UserId: 4) assignments
    (4, 2, '2024-01-01', NULL, GETUTCDATE()),      -- Mobile Banking App (ongoing)
    (4, 4, '2024-03-01', NULL, GETUTCDATE()),      -- Data Analytics Dashboard (ongoing)
    (4, 7, '2024-01-01', NULL, GETUTCDATE()),      -- Training & Development (ongoing)
    
    -- David Lee (UserId: 5) assignments
    (5, 1, '2024-02-01', NULL, GETUTCDATE()),      -- E-Commerce Platform (ongoing)
    (5, 2, '2024-01-01', NULL, GETUTCDATE()),      -- Mobile Banking App (ongoing)
    (5, 8, '2024-01-01', NULL, GETUTCDATE()),      -- Administrative Tasks (ongoing)
    
    -- Lisa Anderson (UserId: 6) assignments
    (6, 4, '2024-01-01', NULL, GETUTCDATE()),      -- Data Analytics Dashboard (ongoing)
    (6, 6, '2024-04-01', NULL, GETUTCDATE()),      -- Customer Portal (ongoing)
    (6, 7, '2024-01-01', NULL, GETUTCDATE());      -- Training & Development (ongoing)
GO

-- =====================================================
-- SEED TIMESHEETS
-- =====================================================
-- Status: 1 = Draft, 2 = Submitted, 3 = Approved, 4 = Rejected

INSERT INTO Timesheets (UserId, SubmissionDate, Status, RejectionComments, CreatedOn)
VALUES
    -- Mike Wilson's timesheets
    (3, '2024-12-16', 3, NULL, GETUTCDATE()),      -- Week Dec 16-20: Approved
    (3, '2024-12-23', 2, NULL, GETUTCDATE()),      -- Week Dec 23-27: Submitted (pending)
    
    -- Emily Brown's timesheets
    (4, '2024-12-16', 3, NULL, GETUTCDATE()),      -- Week Dec 16-20: Approved
    (4, '2024-12-23', 4, 'Hours seem too low for the assigned tasks. Please review and resubmit.', GETUTCDATE()),  -- Week Dec 23-27: Rejected
    
    -- David Lee's timesheets
    (5, '2024-12-16', 3, NULL, GETUTCDATE()),      -- Week Dec 16-20: Approved
    (5, '2024-12-23', 1, NULL, GETUTCDATE()),      -- Week Dec 23-27: Draft
    
    -- Lisa Anderson's timesheets
    (6, '2024-12-16', 3, NULL, GETUTCDATE()),      -- Week Dec 16-20: Approved
    (6, '2024-12-23', 2, NULL, GETUTCDATE());      -- Week Dec 23-27: Submitted (pending)
GO

-- =====================================================
-- SEED TIMESHEET ENTRIES
-- =====================================================

-- Mike Wilson's approved timesheet (Week Dec 16-20)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (1, 1, '2024-12-16', 8, 'Developed shopping cart functionality', GETUTCDATE()),
    (1, 1, '2024-12-17', 7, 'Fixed payment integration bugs', GETUTCDATE()),
    (1, 3, '2024-12-17', 1, 'Team standup and HR system review', GETUTCDATE()),
    (1, 1, '2024-12-18', 8, 'Implemented product search feature', GETUTCDATE()),
    (1, 1, '2024-12-19', 6, 'Code review and refactoring', GETUTCDATE()),
    (1, 7, '2024-12-19', 2, 'Online training - AWS certification', GETUTCDATE()),
    (1, 1, '2024-12-20', 8, 'User acceptance testing support', GETUTCDATE());
GO

-- Mike Wilson's submitted timesheet (Week Dec 23-27)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (2, 1, '2024-12-23', 8, 'Performance optimization', GETUTCDATE()),
    (2, 1, '2024-12-24', 4, 'Half day - Holiday eve', GETUTCDATE()),
    (2, 3, '2024-12-26', 6, 'HR module testing', GETUTCDATE()),
    (2, 7, '2024-12-26', 2, 'Documentation review', GETUTCDATE()),
    (2, 1, '2024-12-27', 8, 'Sprint planning and estimation', GETUTCDATE());
GO

-- Emily Brown's approved timesheet (Week Dec 16-20)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (3, 2, '2024-12-16', 7, 'Mobile app login screen development', GETUTCDATE()),
    (3, 7, '2024-12-16', 1, 'Security training', GETUTCDATE()),
    (3, 2, '2024-12-17', 8, 'Transaction history implementation', GETUTCDATE()),
    (3, 4, '2024-12-18', 4, 'Dashboard requirements analysis', GETUTCDATE()),
    (3, 2, '2024-12-18', 4, 'API integration work', GETUTCDATE()),
    (3, 2, '2024-12-19', 8, 'Push notification feature', GETUTCDATE()),
    (3, 4, '2024-12-20', 6, 'Data visualization prototyping', GETUTCDATE()),
    (3, 7, '2024-12-20', 2, 'Agile methodology workshop', GETUTCDATE());
GO

-- Emily Brown's rejected timesheet (Week Dec 23-27)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (4, 2, '2024-12-23', 4, 'Bug fixes', GETUTCDATE()),
    (4, 4, '2024-12-26', 3, 'Report generation module', GETUTCDATE()),
    (4, 2, '2024-12-27', 4, 'Code cleanup', GETUTCDATE());
GO

-- David Lee's approved timesheet (Week Dec 16-20)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (5, 1, '2024-12-16', 6, 'E-commerce checkout flow', GETUTCDATE()),
    (5, 2, '2024-12-16', 2, 'Banking app code review', GETUTCDATE()),
    (5, 2, '2024-12-17', 8, 'Account management features', GETUTCDATE()),
    (5, 1, '2024-12-18', 8, 'Inventory management module', GETUTCDATE()),
    (5, 2, '2024-12-19', 6, 'Security audit preparation', GETUTCDATE()),
    (5, 8, '2024-12-19', 2, 'Team meeting and admin tasks', GETUTCDATE()),
    (5, 1, '2024-12-20', 8, 'Integration testing', GETUTCDATE());
GO

-- David Lee's draft timesheet (Week Dec 23-27)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (6, 1, '2024-12-23', 8, 'Bug fixing sprint', GETUTCDATE()),
    (6, 2, '2024-12-26', 6, 'Feature development', GETUTCDATE()),
    (6, 8, '2024-12-26', 2, 'Documentation', GETUTCDATE());
GO

-- Lisa Anderson's approved timesheet (Week Dec 16-20)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (7, 4, '2024-12-16', 8, 'Dashboard backend development', GETUTCDATE()),
    (7, 4, '2024-12-17', 6, 'Data pipeline optimization', GETUTCDATE()),
    (7, 7, '2024-12-17', 2, 'Python training', GETUTCDATE()),
    (7, 6, '2024-12-18', 8, 'Customer portal UI design', GETUTCDATE()),
    (7, 4, '2024-12-19', 8, 'Real-time analytics implementation', GETUTCDATE()),
    (7, 6, '2024-12-20', 6, 'Portal authentication module', GETUTCDATE()),
    (7, 7, '2024-12-20', 2, 'Knowledge sharing session', GETUTCDATE());
GO

-- Lisa Anderson's submitted timesheet (Week Dec 23-27)
INSERT INTO TimesheetEntries (TimesheetId, ProjectId, Date, Hours, Description, CreatedOn)
VALUES
    (8, 4, '2024-12-23', 8, 'Chart library integration', GETUTCDATE()),
    (8, 6, '2024-12-24', 4, 'Half day work - UI improvements', GETUTCDATE()),
    (8, 4, '2024-12-26', 6, 'Performance testing', GETUTCDATE()),
    (8, 7, '2024-12-26', 2, 'Year-end review meeting', GETUTCDATE()),
    (8, 6, '2024-12-27', 8, 'Customer portal deployment prep', GETUTCDATE());
GO

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================

SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Projects', COUNT(*) FROM Projects
UNION ALL
SELECT 'ProjectAssignments', COUNT(*) FROM ProjectAssignments
UNION ALL
SELECT 'Timesheets', COUNT(*) FROM Timesheets
UNION ALL
SELECT 'TimesheetEntries', COUNT(*) FROM TimesheetEntries;
GO

PRINT 'Seed data inserted successfully!';
GO
