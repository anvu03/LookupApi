-- Initialize LookupDb Database
-- Add your database initialization scripts here

-- ================================================
-- REFERENCE DATA TABLES
-- ================================================

-- Countries lookup table
CREATE TABLE dbo.Ref_Country (
    IsoCode NVARCHAR(3) PRIMARY KEY,
    CountryName NVARCHAR(255) NOT NULL,
    IsoCode2 NVARCHAR(2),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- States/Provinces lookup table (dependent on Country)
CREATE TABLE dbo.Ref_State (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StateName NVARCHAR(255) NOT NULL,
    StateCode NVARCHAR(10),
    CountryId NVARCHAR(3) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_State_Country FOREIGN KEY (CountryId) REFERENCES dbo.Ref_Country(IsoCode)
);

-- Create indexes for performance
CREATE INDEX IX_Ref_Country_CountryName ON dbo.Ref_Country(CountryName);
CREATE INDEX IX_Ref_State_CountryId ON dbo.Ref_State(CountryId);
CREATE INDEX IX_Ref_State_StateName ON dbo.Ref_State(StateName);

-- ================================================
-- SEED DATA - COUNTRIES
-- ================================================
INSERT INTO dbo.Ref_Country (IsoCode, CountryName, IsoCode2)
VALUES 
    ('USA', 'United States', 'US'),
    ('CAN', 'Canada', 'CA'),
    ('MEX', 'Mexico', 'MX'),
    ('GBR', 'United Kingdom', 'GB'),
    ('DEU', 'Germany', 'DE'),
    ('FRA', 'France', 'FR'),
    ('ITA', 'Italy', 'IT'),
    ('ESP', 'Spain', 'ES'),
    ('AUS', 'Australia', 'AU'),
    ('JPN', 'Japan', 'JP'),
    ('CHN', 'China', 'CN'),
    ('IND', 'India', 'IN'),
    ('BRA', 'Brazil', 'BR');

-- ================================================
-- SEED DATA - STATES (USA)
-- ================================================
INSERT INTO dbo.Ref_State (StateName, StateCode, CountryId)
VALUES 
    ('Alabama', 'AL', 'USA'),
    ('Alaska', 'AK', 'USA'),
    ('Arizona', 'AZ', 'USA'),
    ('Arkansas', 'AR', 'USA'),
    ('California', 'CA', 'USA'),
    ('Colorado', 'CO', 'USA'),
    ('Connecticut', 'CT', 'USA'),
    ('Delaware', 'DE', 'USA'),
    ('Florida', 'FL', 'USA'),
    ('Georgia', 'GA', 'USA'),
    ('Hawaii', 'HI', 'USA'),
    ('Idaho', 'ID', 'USA'),
    ('Illinois', 'IL', 'USA'),
    ('Indiana', 'IN', 'USA'),
    ('Iowa', 'IA', 'USA'),
    ('Kansas', 'KS', 'USA'),
    ('Kentucky', 'KY', 'USA'),
    ('Louisiana', 'LA', 'USA'),
    ('Maine', 'ME', 'USA'),
    ('Maryland', 'MD', 'USA'),
    ('Massachusetts', 'MA', 'USA'),
    ('Michigan', 'MI', 'USA'),
    ('Minnesota', 'MN', 'USA'),
    ('Mississippi', 'MS', 'USA'),
    ('Missouri', 'MO', 'USA'),
    ('Montana', 'MT', 'USA'),
    ('Nebraska', 'NE', 'USA'),
    ('Nevada', 'NV', 'USA'),
    ('New Hampshire', 'NH', 'USA'),
    ('New Jersey', 'NJ', 'USA'),
    ('New Mexico', 'NM', 'USA'),
    ('New York', 'NY', 'USA'),
    ('North Carolina', 'NC', 'USA'),
    ('North Dakota', 'ND', 'USA'),
    ('Ohio', 'OH', 'USA'),
    ('Oklahoma', 'OK', 'USA'),
    ('Oregon', 'OR', 'USA'),
    ('Pennsylvania', 'PA', 'USA'),
    ('Rhode Island', 'RI', 'USA'),
    ('South Carolina', 'SC', 'USA'),
    ('South Dakota', 'SD', 'USA'),
    ('Tennessee', 'TN', 'USA'),
    ('Texas', 'TX', 'USA'),
    ('Utah', 'UT', 'USA'),
    ('Vermont', 'VT', 'USA'),
    ('Virginia', 'VA', 'USA'),
    ('Washington', 'WA', 'USA'),
    ('West Virginia', 'WV', 'USA'),
    ('Wisconsin', 'WI', 'USA'),
    ('Wyoming', 'WY', 'USA');

-- ================================================
-- SEED DATA - PROVINCES (CANADA)
-- ================================================
INSERT INTO dbo.Ref_State (StateName, StateCode, CountryId)
VALUES 
    ('Alberta', 'AB', 'CAN'),
    ('British Columbia', 'BC', 'CAN'),
    ('Manitoba', 'MB', 'CAN'),
    ('New Brunswick', 'NB', 'CAN'),
    ('Newfoundland and Labrador', 'NL', 'CAN'),
    ('Northwest Territories', 'NT', 'CAN'),
    ('Nova Scotia', 'NS', 'CAN'),
    ('Nunavut', 'NU', 'CAN'),
    ('Ontario', 'ON', 'CAN'),
    ('Prince Edward Island', 'PE', 'CAN'),
    ('Quebec', 'QC', 'CAN'),
    ('Saskatchewan', 'SK', 'CAN'),
    ('Yukon', 'YT', 'CAN');

PRINT 'Database initialization completed successfully';
