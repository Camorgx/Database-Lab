CREATE TABLE Book (
    ID CHAR(8),
    name VARCHAR(10) NOT NULL,
    author VARCHAR(10),
    price FLOAT,
    status INT DEFAULT 0,
    borrow_Times INT DEFAULT 0,
    reserve_Times INT DEFAULT 0,
    PRIMARY KEY (ID),
    CHECK (status IN (0, 1, 2))
);

CREATE TABLE Reader (
    ID CHAR(8),
    name VARCHAR(10),
    age INT,
    address VARCHAR(20),
    PRIMARY KEY (ID)
);

CREATE TABLE Borrow (
    book_ID CHAR(8),
    reader_ID CHAR(8),
    borrow_Date DATE,
    return_Date DATE,
    PRIMARY KEY (book_ID, reader_ID, borrow_Date),
    FOREIGN KEY (book_ID) REFERENCES Book(ID),
    FOREIGN KEY (reader_ID) REFERENCES Reader(ID)
);

CREATE TABLE Reserve (
    book_ID CHAR(8),
    reader_ID CHAR(8),
    reserve_Date DATE DEFAULT (CURDATE()),
    take_Date DATE,
    PRIMARY KEY (book_ID, reader_ID, reserve_Date),
    FOREIGN KEY (book_ID) REFERENCES Book(ID),
    FOREIGN KEY (reader_ID) REFERENCES Reader(ID),
    CHECK (take_Date > reserve_Date)
);
