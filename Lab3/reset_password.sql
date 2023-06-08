drop procedure if exists resetPassword;
delimiter //
create procedure resetPassword(
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
    if ID not in (select teacherID from TeacherAccount) then set result = 1;
    elseif verify != (select verification from TeacherAccount where teacherID = ID)
        then set result = 2;
    else
        update TeacherAccount set password = pwd where teacherID = ID;
    end if;
    if s = 0 then commit;
    else
        set result = 3;
        rollback;
    end if;
end //
delimiter ;
