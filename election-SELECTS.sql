/* All of the following SELECT statements are for the requirements */

/* 1. List all candidates with more than 20 first preference votes */
SELECT DISTINCT p1.cand_id, count(*) as Votes FROM Preferences AS p1 WHERE 20 < (SELECT DISTINCT count(*) FROM Preferences AS p2 WHERE p1.cand_id = p2.cand_id AND prefer = '1') AND p1.elec_id = 'E02' AND p1.prefer = '1' GROUP BY p1.cand_id;
/* SELECT DISTINCT p1.cand_id FROM Preferences AS p1 WHERE 2000 < (SELECT count(*) FROM Preferences AS p2 WHERE p1.cand_id = p2.cand_id); */

/* 2. List all candidates standing in the electorate of “Bedford Park” */
SELECT DISTINCT cand_id, firstname_c, surname_c  FROM Candidates AS p1 WHERE elector_id IN (SELECT elector_id FROM Electorate WHERE elector_name = 'Bedford Park') GROUP BY cand_id ORDER BY surname_c ASC;

/* 3. List all voters eligible to vote in the electorate of “Bedford Park”. */
SELECT DISTINCT voter_id, firstname_v, surname_v FROM Voters WHERE elector_id = 'BFP' GROUP BY voter_id ORDER BY surname_v ASC;

/* 4. List all candidates by electorate with their first preference counts. */
SELECT DISTINCT elector_id, cand_id, house_id, count(*) AS 'First_Pref' FROM Preferences WHERE prefer = 1 AND elec_id = 'E02' GROUP BY cand_id ORDER BY First_Pref DESC;

/* 5. List all voters who were eligible to vote in the electorate of ”Bedford Park” but didn’t.*/
SELECT DISTINCT voter_id, firstname_v, surname_v FROM Voters WHERE elector_id = 'BFP' AND voter_id IN (SELECT voter_id FROM HasVoted WHERE voted = 0 AND elec_id = 'E02') GROUP BY voter_id ORDER BY surname_v ASC;

/* 6. List the name and address of all candidates who did not get more than 3 votes */
SELECT DISTINCT p1.firstname_c, p1.surname_c, p1.address_c FROM Candidates AS p1 WHERE 3 > (SELECT DISTINCT count(*) FROM Preferences AS p2 WHERE p1.cand_id = p2.cand_id AND p2.elec_id = 'E02' AND p2.prefer = '1') GROUP BY p1.cand_id ORDER BY p1.surname_c;

/* 7. List all the candidates who received fewer votes than “Ellen”. */
SELECT p1.cand_id, count(*) as Votes FROM Preferences AS p1 WHERE p1.elec_id = 'E02' GROUP BY p1.cand_id HAVING Votes < (SELECT count(*) FROM Preferences AS p3 WHERE p3.cand_id = 'JHK' AND p3.prefer = '1' GROUP BY p3.cand_id) ORDER BY Votes DESC;

/* 8. List all electorates, in alphabetical order, who have four or more candidates standing. */
SELECT DISTINCT p1.elector_id, p1.elector_name FROM Electorate AS p1 WHERE 4 <= (SELECT DISTINCT count(*) FROM Candidates AS p2 WHERE p1.elector_id = p2.elector_id) GROUP BY p1.elector_id;

/* 9. List all candidates, in alphabetical order, who received no votes. */
SELECT DISTINCT cand_id, firstname_c, surname_c FROM Candidates WHERE cand_id NOT IN (SELECT cand_id FROM Preferences WHERE elec_id = 'E02' AND prefer = '1') GROUP BY cand_id ORDER BY firstname_c ASC;

/* 10. Change the name of the ”Bedford Park” electorate to “Flinders University */
UPDATE Electorate SET elector_name = 'Flinders University' WHERE elector_name = 'Bedford Park';

/* 11. List all candidates that have stood for more than one election. */
SELECT DISTINCT p1.cand_id, p1.firstname_c, p1.surname_c FROM Candidates as p1 WHERE 2 <= (SELECT count(*) FROM Elections AS p2 WHERE p1.cand_id = p2.cand_id) GROUP BY p1.cand_id ORDER BY p1.surname_c;

/* 12. Display the party with the most seats */
SELECT DISTINCT p1.party_id, p1.party_name, MAX((SELECT count(*) FROM Seats AS p2 WHERE p1.party_id = p2.party_id AND elec_id = 'E02')) FROM Parties AS p1;

/* SECURITY STATEMENT */
SELECT DISTINCT elec_id, count(*) AS 'Votes' FROM Preferences WHERE Prefer = 1 AND house_id = 0 AND elec_id = 'E02' UNION SELECT DISTINCT elec_id, count(*) AS 'Voters' FROM HasVoted WHERE elec_id = 'E02' AND voted = 1;
