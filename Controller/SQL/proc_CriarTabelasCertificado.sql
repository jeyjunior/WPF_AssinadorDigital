USE [GECertificados]
GO

/****** Object:  StoredProcedure [dbo].[CriarTabelaCertificados]    Script Date: 17/01/2024 22:10:44 ******/
DROP PROCEDURE [dbo].[CriarTabelaCertificados]
GO

/****** Object:  StoredProcedure [dbo].[CriarTabelaCertificados]    Script Date: 17/01/2024 22:10:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CriarTabelaCertificados]

AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Certificados')
    BEGIN
        CREATE TABLE TCertificado
		(
			PK_Certificado INT IDENTITY(1,1) PRIMARY KEY,
			NomeCertificado NVARCHAR(50) NOT NULL,
			CPF NVARCHAR(11) NOT NULL,
			Email NVARCHAR(100),
			Senha NVARCHAR(50) NULL,
			ChavePrivada VARBINARY(MAX) NULL,
			ChavePublica NVARCHAR(MAX) NULL,
			Emissor NVARCHAR(30) NULL,
			EmissorTipoO NVARCHAR(20) NULL,
			DataValidade DATE NULL,
			Certificado VARBINARY(MAX) NULL,
		);
    END
END;
GO


