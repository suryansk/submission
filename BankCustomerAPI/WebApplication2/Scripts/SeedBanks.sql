-- Sample Banks for Testing Manager User Registration
USE training;

-- Insert sample banks if they don't exist
IF NOT EXISTS (SELECT 1 FROM training.Banks WHERE BankCode = 'SBI001')
BEGIN
    INSERT INTO training.Banks (Name, BankCode, SwiftCode, HeadOfficeAddress, ContactNumber, Email, IsActive, EstablishedDate)
    VALUES 
    ('State Bank of India', 'SBI001', 'SBININBB', 'Mumbai, Maharashtra, India', '+91-22-22740000', 'customer.care@sbi.co.in', 1, '1955-07-01'),
    ('HDFC Bank', 'HDFC001', 'HDFCINBB', 'Mumbai, Maharashtra, India', '+91-22-66316000', 'customercare@hdfcbank.com', 1, '1994-08-30'),
    ('ICICI Bank', 'ICICI001', 'ICICINBB', 'Mumbai, Maharashtra, India', '+91-22-26534200', 'customer.care@icicibank.com', 1, '1994-05-05'),
    ('Axis Bank', 'AXIS001', 'AXISINBB', 'Mumbai, Maharashtra, India', '+91-22-24251414', 'customer.care@axisbank.com', 1, '1993-12-03'),
    ('Punjab National Bank', 'PNB001', 'PUNBINBB', 'New Delhi, India', '+91-11-23715150', 'customercare@pnb.co.in', 1, '1894-05-19');
END

-- Display the banks
SELECT Id, Name, BankCode, SwiftCode FROM training.Banks;