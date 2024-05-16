--===================================================================== SỐ ĐẠI LÝ TỐI ĐA TRONG QUẬN ======================================================================--
--Số đại lý tối đa
CREATE TRIGGER DAILY_MaxDaiLyTrigger
ON DAILY
AFTER INSERT, UPDATE
AS
BEGIN
    -- Kiểm tra số lượng đại lý trong mỗi quận
    IF EXISTS (
        SELECT d.MaQuan
        FROM DAILY d
        INNER JOIN QUAN q ON d.MaQuan = q.MaQuan
        INNER JOIN inserted i ON i.MaQuan = q.MaQuan
        GROUP BY d.MaQuan
        HAVING COUNT(*) > (SELECT SoDaiLyToiDa FROM THAMSO)
    )
    BEGIN
        -- Nếu vượt quá số đại lý tối đa trong quận
        PRINT(N'Vượt quá số đại lý tối đa của quận')
        ROLLBACK
        RETURN
    END
END;

--============================================================ NHẬP HÀNG ==============================================================--
-- Cập nhật số lượng tồn
CREATE TRIGGER CT_PNH_CapNhatSoLuongTon
ON CT_PNH
AFTER INSERT, DELETE
AS
BEGIN
    -- Cập nhật giá trị SoLuongTon trong bảng MATHANG
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
        -- INSERT
        UPDATE MATHANG
        SET SoLuongTon = MATHANG.SoLuongTon + i.SoLuongNhap
        FROM MATHANG
        INNER JOIN inserted i ON MATHANG.MaMH = i.MaMH;
    END
    ELSE
    BEGIN
        -- DELETE
        UPDATE MATHANG
        SET SoLuongTon = MATHANG.SoLuongTon - d.SoLuongNhap
        FROM MATHANG
        INNER JOIN deleted d ON MATHANG.MaMH = d.MaMH;
    END
END;



-- Tính đơn giá nhập xuất cho mặt hàng
CREATE TRIGGER CT_PNH_CapNhatDonGia
ON CT_PNH
AFTER INSERT, UPDATE
AS
BEGIN
    -- Cập nhật DonGiaNhap của MATHANG
    UPDATE MATHANG
    SET DonGiaNhap = i.DonGiaNhap
    FROM MATHANG m
    INNER JOIN inserted i ON m.MaMH = i.MaMH;
    
    -- Cập nhật DonGiaXuat của MATHANG
    UPDATE MATHANG
    SET DonGiaXuat = m.DonGiaNhap/100 * (SELECT TiLeDonGia FROM THAMSO) 
    FROM MATHANG m
    INNER JOIN inserted i ON m.MaMH = i.MaMH
END;

--============================================================ XUẤT HÀNG ========================================================================--
--Số lượng tồn
CREATE TRIGGER UpdateSoLuongTon
ON CT_PXH
AFTER INSERT, DELETE
AS
BEGIN
    -- Cập nhật số lượng tồn trong bảng MATHANG sau khi xuất hàng
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        -- Thực hiện khi có lệnh DELETE được thực thi
        UPDATE MATHANG
        SET SoLuongTon = MATHANG.SoLuongTon + deleted.SoLuongXuat
        FROM MATHANG
        INNER JOIN deleted ON MATHANG.MaMH = deleted.MaMH
        WHERE MATHANG.MaMH IN (SELECT MaMH FROM deleted);
    END
    ELSE
    BEGIN
        -- Thực hiện khi có lệnh INSERT được thực thi
        UPDATE MATHANG
        SET SoLuongTon = MATHANG.SoLuongTon - inserted.SoLuongXuat
        FROM MATHANG
        INNER JOIN inserted ON MATHANG.MaMH = inserted.MaMH
        WHERE MATHANG.MaMH IN (SELECT MaMH FROM inserted);
    END
END;


-- Kiểm tra số tiền nợ
CREATE TRIGGER KiemTraSoTienNo
ON PHIEUXUATHANG
AFTER INSERT, DELETE
AS
BEGIN
    -- Kiểm tra và ngăn chặn việc tạo phiếu xuất hàng nếu Tiền Nợ vượt quá Số Nợ Tối Đa
    IF EXISTS (
        SELECT DAILY.MaDaiLy
        FROM DAILY
        INNER JOIN LOAIDAILY ON DAILY.MaLoaiDaiLy = LOAIDAILY.MaLoaiDaiLy
        INNER JOIN inserted ON DAILY.MaDaiLy = inserted.MaDaiLy
        WHERE (DAILY.TienNo + inserted.ConLai) > LOAIDAILY.SoNoToiDa
    )
    BEGIN
        -- Hiển thị thông báo lỗi
        PRINT (N'Không thể tạo phiếu xuất hàng. Tiền Nợ vượt quá Số Nợ Tối Đa')
		ROLLBACK TRAN
    END
    ELSE
    BEGIN
        -- Cập nhật Tiền Nợ (TienNo) trong bảng DAILY
        IF EXISTS (SELECT * FROM deleted)
        BEGIN
            -- Thực hiện khi có lệnh DELETE được thực thi
            UPDATE DAILY
            SET TienNo = DAILY.TienNo - deleted.ConLai
            FROM DAILY
            INNER JOIN deleted ON DAILY.MaDaiLy = deleted.MaDaiLy;
        END
        ELSE
        BEGIN
            -- Thực hiện khi có lệnh INSERT được thực thi
            UPDATE DAILY
            SET TienNo = DAILY.TienNo + inserted.ConLai
            FROM DAILY
            INNER JOIN inserted ON DAILY.MaDaiLy = inserted.MaDaiLy;
        END
    END
END;



--============================================================Phiếu thu tiền================================================================================--

CREATE TRIGGER PHIEUTHUTIEN_TinhTienNo
ON PHIEUTHUTIEN
AFTER INSERT, DELETE
AS
BEGIN
    -- Kiểm tra và ngăn chặn việc thu tiền nếu số tiền thu vượt quá số tiền nợ
    IF EXISTS (
        SELECT PTT.MaDaiLy
        FROM PHIEUTHUTIEN PTT
        INNER JOIN DAILY DL ON PTT.MaDaiLy = DL.MaDaiLy
        WHERE PTT.MaPhieuThuTien IN (SELECT MaPhieuThuTien FROM inserted)
        AND PTT.SoTienThu > DL.TienNo
    )
    BEGIN
        -- Số tiền thu vượt quá số tiền nợ, thông báo lỗi
        PRINT(N'Số tiền thu vượt quá số tiền nợ')
        ROLLBACK
        RETURN
    END

    -- Cập nhật số tiền nợ trong bảng DAILY sau khi thu tiền
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        -- Thực hiện khi có lệnh DELETE được thực thi
        UPDATE DAILY
        SET TienNo = TienNo + (SELECT SUM(PTT.SoTienThu) FROM deleted PTT WHERE DAILY.MaDaiLy = PTT.MaDaiLy)
        FROM DAILY
        INNER JOIN deleted ON DAILY.MaDaiLy = deleted.MaDaiLy;
    END
    ELSE
    BEGIN
        -- Thực hiện khi có lệnh INSERT được thực thi
        UPDATE DAILY
        SET TienNo = TienNo - (SELECT SUM(PTT.SoTienThu) FROM inserted PTT WHERE DAILY.MaDaiLy = PTT.MaDaiLy)
        FROM DAILY
        INNER JOIN inserted ON DAILY.MaDaiLy = inserted.MaDaiLy;
    END
END;


--=====================================================================BÁO CÁO DOANH SỐ======================================================================--
CREATE PROCEDURE CalculateAndDisplayReport
    @Thang int,
    @Nam int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaBCDS char(6);
    SET @MaBCDS = 'B' + RIGHT('00' + CAST(@Thang AS VARCHAR(2)), 2) + RIGHT(CAST(@Nam AS VARCHAR(4)), 2);

    -- Kiểm tra xem báo cáo đã tồn tại hay chưa
    IF NOT EXISTS (SELECT 1 FROM BAOCAODOANHSO WHERE MaBCDS = @MaBCDS)
    BEGIN
        -- Tính toán tổng doanh thu và lưu vào bảng BAOCAODOANHSO
        DECLARE @TongDoanhThu money;

        SELECT @TongDoanhThu = SUM(p.TongTien)
        FROM PHIEUXUATHANG p
        INNER JOIN DAILY d ON p.MaDaiLy = d.MaDaiLy
        WHERE YEAR(p.NgayXuat) = @Nam AND MONTH(p.NgayXuat) = @Thang;

        INSERT INTO BAOCAODOANHSO (MaBCDS, Thang, Nam, TongDoanhThu)
        VALUES (@MaBCDS, @Thang, @Nam, @TongDoanhThu);

        -- Tính toán và lưu CT_BCDS
        INSERT INTO CT_BCDS (MaBCDS, MaDaiLy, SoPhieuXuat, TongTriGia, TyLe)
        SELECT @MaBCDS, d.MaDaiLy, COUNT(p.MaPhieuXuat), SUM(p.TongTien), SUM(p.TongTien) / @TongDoanhThu
        FROM PHIEUXUATHANG p
        INNER JOIN DAILY d ON p.MaDaiLy = d.MaDaiLy
        WHERE YEAR(p.NgayXuat) = @Nam AND MONTH(p.NgayXuat) = @Thang
        GROUP BY d.MaDaiLy;
    END
END;


EXEC CalculateAndDisplayReport @Thang = 6, @Nam = 2023;

--=====================================================================BÁO CÁO CÔNG NỢ======================================================================--
CREATE PROCEDURE Report_BAOCAOCONGNO
    @Thang int,
    @Nam int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaBCCN char(6);
    SET @MaBCCN = 'N' + RIGHT('00' + CAST(@Thang AS VARCHAR(2)), 2) + RIGHT(CAST(@Nam AS VARCHAR(4)), 2);

    -- Kiểm tra xem báo cáo đã tồn tại hay chưa
    IF NOT EXISTS (SELECT 1 FROM BAOCAOCONGNO WHERE MaBCCN = @MaBCCN)
    BEGIN
        -- Tạo báo cáo công nợ mới
        INSERT INTO BAOCAOCONGNO (MaBCCN, Thang, Nam)
        VALUES (@MaBCCN, @Thang, @Nam);

        -- Nợ đầu của mỗi đại lý trong tháng này
        INSERT INTO CT_BCCN (MaBCCN, MaDaiLy, NoDau, PhatSinh, NoCuoi)
        SELECT @MaBCCN, MaDaiLy, ISNULL((SELECT NoCuoi FROM CT_BCCN WHERE MaBCCN = 'N' + RIGHT('00' + CAST(@Thang - 1 AS VARCHAR(2)), 2) + RIGHT(CAST(@Nam AS VARCHAR(4)), 2) AND MaDaiLy = DAILY.MaDaiLy), 0), 0, 0
        FROM DAILY
        WHERE EXISTS (
            SELECT 1
            FROM PHIEUXUATHANG pxh
            WHERE YEAR(pxh.NgayXuat) = @Nam AND MONTH(pxh.NgayXuat) = @Thang AND pxh.MaDaiLy = DAILY.MaDaiLy
        ) OR EXISTS (
            SELECT 1
            FROM PHIEUTHUTIEN ptt
            WHERE YEAR(ptt.NgayThuTien) = @Nam AND MONTH(ptt.NgayThuTien) = @Thang AND ptt.MaDaiLy = DAILY.MaDaiLy
        );

        -- Cập nhật phát sinh của mỗi đại lý trong tháng
        UPDATE CT_BCCN
        SET PhatSinh = (
            SELECT SUM(ConLai)
            FROM PHIEUXUATHANG
            WHERE YEAR(NgayXuat) = @Nam AND MONTH(NgayXuat) = @Thang AND MaDaiLy = CT_BCCN.MaDaiLy
        )
        WHERE MaBCCN = @MaBCCN;

        -- Cập nhật nợ cuối của mỗi đại lý trong tháng
        UPDATE CT_BCCN
        SET NoCuoi = NoDau + PhatSinh - (
            SELECT ISNULL(SUM(SoTienThu), 0)
            FROM PHIEUTHUTIEN
            WHERE YEAR(NgayThuTien) = @Nam AND MONTH(NgayThuTien) = @Thang AND MaDaiLy = CT_BCCN.MaDaiLy
        )
        WHERE MaBCCN = @MaBCCN;

        -- Tính tổng nợ của một tháng
        DECLARE @TongNo money;
        SELECT @TongNo = SUM(NoCuoi)
        FROM CT_BCCN
        WHERE MaBCCN = @MaBCCN;

        -- Cập nhật tổng nợ vào bảng BAOCAOCONGNO
        UPDATE BAOCAOCONGNO
        SET TongNo = @TongNo
        WHERE MaBCCN = @MaBCCN;
    END;

    -- Hiển thị thông tin báo cáo công nợ của các đại lý có phiếu xuất hoặc phiếu thu trong tháng
    SELECT c.MaBCCN, d.MaDaiLy, d.TenDaiLy, c.NoDau, c.PhatSinh, c.NoCuoi
    FROM CT_BCCN c
    INNER JOIN DAILY d ON c.MaDaiLy = d.MaDaiLy
    WHERE c.MaBCCN = @MaBCCN
        AND (EXISTS (
                SELECT 1
                FROM PHIEUXUATHANG pxh
                WHERE YEAR(pxh.NgayXuat) = @Nam AND MONTH(pxh.NgayXuat) = @Thang AND pxh.MaDaiLy = d.MaDaiLy
            ) OR EXISTS (
                SELECT 1
                FROM PHIEUTHUTIEN ptt
                WHERE YEAR(ptt.NgayThuTien) = @Nam AND MONTH(ptt.NgayThuTien) = @Thang AND ptt.MaDaiLy = d.MaDaiLy
            ));
END;


--=============================================================================================================================================--


