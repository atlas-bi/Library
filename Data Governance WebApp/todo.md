
# mail

* Send attachements
* Add folders. Folders will be mostly a copy from the favorite folders.
* Js function to allow keyboard use to go between/delete mails
* Add sorting
* Enable C# SMTP > notifications to view shared stuff, in atlas or epic
* Allow bold/italics etc in editor
* Conversation grouping
* Make it more extendable (ie, separate sections for reader, writer, etc)
* are dup users coming up?
* user dropdown > add  remove user & split group to users
* show marker that you have already replied/forwarded an email
* right now the only message is “mail sent”, even for errors.. need to add in error messages
* Add atlas section for groups > similar to users.
* allow delete draft, but disable forward/reply
* make dynamic content look ok until js is loaded
* fix multi select hight in report editor
* save last state and load when mail is accessed

# bugs

* terms homepage needs facelift
* update chart hovering colors
* search cannot find AR, OR - (no proceded or followed by letter)
* when clicking breadcrumbs for a search the box is not showing
* cannot select to recips on new mail
* dropdown on mail to is overflowing edges

# todo

* test new EPIC browser that will pass user ID into site
* update projects editor
* update terms editor
* update initiatives editor
* add back search fav button
* notification of edit to a fav
* Version tracking within the app
	> verioning code/etl'd stuff? automated change log of all report data
* Showing where in ReportObject/ReportObject_doc the search term is matching in the search results
* Adding Search term aliases via menu. Aliases would be things like ‘Inpatient’ for ‘IP’ or ‘Emergency’ for ED
	> this will be an app dictionary that search references
	> tag on users. so if a user has run a report with "OB" tag on them, we find the other reports they have

* Incorporating run data into search results ranking
* Incorporating request process and queue ranking
* Incorporating extracts
* Allow report requests
* Move email subscriptions into Atlas
* Ability to run sql code/ SSRS report directly in the app
* can reports be grouped (copies of etc.)
* data qualiy monitoring
	> things the break reporting
	> making reporting easier
  for example, how do we find all reports that may have a problem with change in observation? need a better way to find reports related to observation etc.
	sql - we can find problem pieces.. need advanced search stuff