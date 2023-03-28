
school_acronyms = ['SBVC', 'CSULA', 'CCC', 'UCB']
school_names = ['San Bernardino Valley College', 'Cal State University Los Angeles', 'Contra Costa College', 'University of California Berkeley']

print("Enter a school you would like to see, your choices are San Bernardino Valley College, Cal State University Los Angeles, Contra Costa College,and University of California Berkeley: ") 

IndexInput = input("Which school would you like to see? The 1st ,2nd, 3rd, or 4th?: ") 

indexNumber = int(IndexInput) - 1  

schoolAcronym = school_acronyms[indexNumber]  

schoolName = school_names[indexNumber]  

print("Your results are School Acronym: " + schoolAcronym + " and School Name: " + schoolName) 