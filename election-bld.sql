DROP TABLE IF EXISTS Parties;
DROP TABLE IF EXISTS Electorate;
DROP TABLE IF EXISTS Voters;
DROP TABLE IF EXISTS Candiates;
DROP TABLE IF EXISTS Elections;
DROP TABLE IF EXISTS Seats;
DROP TABLE IF EXISTS HasVoted;
DROP TABLE IF EXISTS Preferences;

CREATE TABLE Parties (
party_id CHAR(4) NOT NULL,
party_name CHAR(30) NOT NULL,
PRIMARY KEY (party_id)
);

CREATE TABLE Electorate (
elector_id CHAR(4) NOT NULL,
elector_name char(30) NOT NULL,
PRIMARY KEY (elector_id)
);

CREATE TABLE Voters (
voter_id CHAR(6) NOT NULL,
elector_id CHAR(4) NOT NULL,
firstname_v CHAR(30) NOT NULL,
surname_v CHAR(30) NOT NULL,
address_v CHAR(30) NOT NULL,
postcode CHAR(4) NOT NULL,
contactphone_v CHAR(14) NOT NULL,
PRIMARY KEY (voter_id, elector_id),
FOREIGN KEY (elector_id) REFERENCES Electorate (elector_id) ON UPDATE CASCADE
);

CREATE TABLE Candidates (
cand_id CHAR(3) NOT NULL,
elector_id CHAR(4) NOT NULL,
firstname_c CHAR(30) NOT NULL,
surname_c CHAR(30) NOT NULL,
address_c CHAR(30) NOT NULL,
PRIMARY KEY (cand_id),
FOREIGN KEY (elector_id) REFERENCES Electorate (elector_id) ON UPDATE CASCADE
);

CREATE TABLE Elections (
elec_id CHAR(4) NOT NULL,
house_id INT(1) NOT NULL,
SDATE DATE NOT NULL,
party_id CHAR(4) NOT NULL,
cand_id CHAR(3) NOT NULL,
PRIMARY KEY (elec_id, house_id, cand_id),
FOREIGN KEY (party_id) REFERENCES Parties (party_id) ON UPDATE CASCADE,
FOREIGN KEY (cand_id) REFERENCES Candidates (cand_id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Seats (
seat_id CHAR(4) NOT NULL,
elector_id CHAR(4) NOT NULL,
house_id INT(1) NOT NULL,
elec_id CHAR(4) NOT NULL,
cand_id CHAR(3) NOT NULL,
party_id CHAR(4) NOT NULL,
PRIMARY KEY (seat_id, elector_id, elec_id, cand_id),
FOREIGN KEY (elector_id) REFERENCES Electorate (elector_id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY (house_id) REFERENCES Elections (house_id) ON UPDATE CASCADE,
FOREIGN KEY (elec_id) REFERENCES Elections (elec_id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY (cand_id) REFERENCES Candidates (cand_id) ON UPDATE CASCADE,
FOREIGN KEY (party_id) REFERENCES Parties (party_id) ON UPDATE CASCADE
);

CREATE TABLE HasVoted (
voter_id CHAR(6) NOT NULL,
elec_id CHAR(4) NOT NULL,
voted INT(1) NOT NULL,
PRIMARY KEY (voter_id, elec_id),
FOREIGN KEY (voter_id) REFERENCES Voters (voter_id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY (elec_id) REFERENCES Elections (elec_id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE Preferences (
id INT AUTO_INCREMENT,
elector_id CHAR(4) NOT NULL,
elec_id CHAR(4) NOT NULL,
cand_id CHAR(3) NOT NULL,
house_id INT(1) NOT NULL,
prefer CHAR(2) NOT NULL,
PRIMARY KEY (id, elec_id, cand_id, house_id),
FOREIGN KEY (elector_id) REFERENCES Electorate (elector_id) ON UPDATE CASCADE,
FOREIGN KEY (elec_id) REFERENCES Elections (elec_id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY (cand_id) REFERENCES Candidates (cand_id) ON UPDATE CASCADE,
FOREIGN KEY (house_id) REFERENCES Elections (house_id) ON UPDATE CASCADE
);
