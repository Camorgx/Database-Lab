drop procedure if exists register;
delimiter //
create procedure register(
    in ID char(5),
    in pwd varchar(20),
    in verify varchar(20),
    out result int
)
begin
    declare s int default 0;
    declare continue handler for sqlexception set s = 1;
    start transaction;
    set result = 0;
    if ID in (select teacherID from teacheraccount) then set result = 1;
    else
        insert into Teacher value (ID, null, null, null);
        insert into TeacherAccount value (ID, pwd, verify);
    end if;
    if s = 0 then commit;
    else
        set result = 2;
        rollback;
    end if;
end //
delimiter ;
