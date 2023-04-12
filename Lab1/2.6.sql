SELECT
    ID AS '读者号',
    name AS '姓名'
FROM Reader
WHERE
    ID NOT IN (
        SELECT DISTINCT Reader.ID AS ID
        FROM Reader, Borrow
        WHERE
            Reader.ID = Borrow.reader_ID
            AND Borrow.book_ID IN 
                (SELECT ID FROM Book WHERE author = 'John'));
