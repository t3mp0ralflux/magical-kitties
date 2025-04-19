begin transaction;
insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, activated_utc, last_login_utc, deleted_utc, account_status, account_role)
values('77bd9552-2153-4e72-ad3d-5e0316db6253', 'MagicalKitties', 'Service', 'magicalkittyservice', 'brenton.belanger@gmail.com', '9A003222166F23043D6A97A77AF9D2166838EE3ABDFE65D96169F31E0AD6123E-73506780E1D56922443A9D01BB7D2293', '2025-03-23T09:40:00', '2025-03-23T09:40:00', '2025-03-23T09:40:00', null, null, 1, 0);

insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, last_login_utc, deleted_utc, activated_utc, account_status, account_role)
values('4174494b-9d60-4d11-bb4a-eff736cc5bf8', 'Brent', 'Belanger', 't3mp0ralflux', 't3mp0ralflux@gmail.com', '9A003222166F23043D6A97A77AF9D2166838EE3ABDFE65D96169F31E0AD6123E-73506780E1D56922443A9D01BB7D2293', '2025-03-23T09:40:00', '2025-03-23T09:40:00', '2025-03-23T09:40:00', null, '2025-03-23T09:05:00', 1, 0);

insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, last_login_utc, deleted_utc, activated_utc, account_status, account_role)
values('b95a3fb7-368d-47e2-9296-d6b60e9073b9', 'Big', 'Chungus', 'bigchungus', 'bigchungus@meme.com', '6E008EA9461D8BAB01B70F4F8D50D3242F69C187FB4837594CAE81C9BE647186-9EB5DD5BDC3CDFFAB33DFEA6EDBEDB50', '2025-03-24T09:40:00', '2025-03-24T09:40:00', '2025-03-24T09:40:00', null, '2025-03-23T09:05:00', 0, 1);

insert into globalsetting(id, name, value)
values('a8c45217-1a04-4c42-9989-cefb5cdf09a2', 'service_account_username', 'magicalkittyservice');

insert into globalsetting(id, name, value)
values('545c30a9-f23d-4366-9c7f-10d7b24c4700', 'activation_email_format', '<p>Your activation link is:</p><p>{0}</p><p>For security reasons, do not share this link with anyone. This link will stop working after {1} minutes.</p><p>To keep future security messages like this from going to spam or junk email, Add no-reply@mail.magicalkitties.com to your approved or safe sender list.</p><p>If you did not make this request, contact us. Please do not respond to this email.</p>')

commit;

begin transaction;
insert into flaw(id, name, description, is_custom)
values('11', 'Amnesia', 'You are missing a part of your past, having lost some or all of your memory.', false);
values('12', 'Arrogant', 'You think you can do anything, even when it\'s clearly beyond your ability.', false);
values('13', 'Big Mouth', 'You talk a lot and often say things when you shouldn\'t, or reveal things that should be kept secret.', false);
values('14', 'Careless', 'You are clumsy and inattentive. You often break things, perhaps because you don\'t know your own strength.', false);
values('15', 'Cry Baby', 'Anything remotely sad makes you break down in tears.', false);
values('16', 'Distractable', 'You are easily sidetracked by shiny things.', false);
values('21', 'Dizziness', 'Sometimes, things seem to spin around you when they shouldn\'t.', false);
values('22', 'Forgetful', 'Often, you don\'t remember important information.', false);
values('23', 'Gluttonous', 'You really like food. Even when you\'ve just eaten, you find it hard to resist as quick bite.', false);
values('24', 'Greedy', 'You want things, and you think you deserve more stuff than everyone else.', false);
values('25', 'Grumpy', 'You are often in a bad mood for no reason.', false);
values('26', 'Gullible', 'You are too innocent and trusting. You believe just about anythign somebody tells you.', false);
values('31', 'Hallucinations', 'You often see or hear things that are not really there.', false);
values('32', 'Hyperactive', 'You are full of boundless energey, and aren\'t satisfied unless you\'re doing something.', false);
values('33', 'Impulsive', 'You tend to rush into things without thinking them through.', false);
values('34', 'Indecisive', 'You have a hard time making up your mind, and then worry you made the wrong decision.', false);
values('35', 'Jumps to Conclusions', 'You often assume you know what\'s going on, even when you don\'t.', false);
values('36', 'Kittylexia', 'You can\'t read human languages at all.', false);
values('41', 'Lazy', 'You always prefer taking the easy way, or making someone else work for you.', false);
values('42', 'Loud', 'You make a lot of noise, often at the worst possible time.', false);
values('43', 'Nosey', 'You havea thirst for knowledge and are eager to investigate anything unusual, even if this annoys others.', false);
values('44', 'Obsessive Collector', 'There is a specific item (or type of item) that you simply <i>must</i> have.', false);
values('45', 'Overactive Imagination', 'You frequently imagine things to be far different than what they really are.', false);
values('46', 'Paranoid', 'You worry that everyone is plotting against you.', false);
values('51', 'Pessimistic', 'You see the worst in everything. You know god times can\'t last, and bad times always get worse.', false);
values('52', 'Scaredy Cat', 'You are mildly afraid of just about everything, or extremely afraid of one specific thing.', false);
values('53', 'Secrets', 'You have a really important secret (or many secrets) that you are keeping from your friends.', false);
values('54', 'Show-Off', 'You always feel like you need to prove how awesome you are.', false);
values('55', 'Shy', 'You feel awkward around other people, and have a hard time talking to strangers.', false);
values('56', 'Sleepy', 'You really like taking naps, and sleep whenever you have the choice.', false);
values('61', 'Snobby', 'You are conceited, always looking down on people who are not as good as you.', false);
values('62', 'Soft-Hearted', 'You feel compassion for those in danger or discomfort, and are willing to help them in any way you can.', false);
values('63', 'Squeamish', 'You feel faint or sick when confronted with anything that\'s totally gross.', false);
values('64', 'Stubborn', 'Your dogged determination means you refuse to change your mind.', false);
values('65', 'Superstitious', 'You believe in myths, urban legends, and bad luck.', false);
values('66', 'Vanity', 'You are very proud of your appearance. Grooming and presentation are very important to you.', false);

commit;

begin transaction;
insert into flaw(id, name, description, is_custom)
values('11', 'Animal Friend', 'You like and get long with squirrels, robins, and other non-magical critters. Even dogs, most of the time.', false);
values('12', 'Artistic', 'You are good at visual arts like painting and sculpting.', false);
values('13', 'Athletic', 'You are good at physical activities like running, jumping, and climbing.', false);
values('14', 'Balance', 'You have a great sense of balance and can easily walk along a tightrope, window ledge, or tree branch.', false);
values('15', 'Bargainer', 'You\'re good at getting what you want, but even more importantly you enjoy helping people who are arguing come to an agreement.', false);
values('16', 'Big Kitty', 'You\'re larger than most other kitties, and you can throw your weight around.', false);
values('21', 'Calculator', 'You understand numbers and math easily, and you like applying them to real life.', false);
values('22', 'Claws', 'You are very proud of your razor-sharp claws, and can use them in all sorts of clever ways.', false);
values('23', 'Dancer', 'You know how to move to music and get your groove on. Your dancing looks good, and it\'s fun, too!', false);
values('24', 'Daredevil', 'You love taking really big risks and leaping into danger.', false);
values('25', 'Dramatist', 'You\'re a talented actor who can spin a good story that entertains others.', false);
values('26', 'Empathic', 'You have a knack for understanding the feelings of others.', false);
values('31', 'Escape Artist', 'You know how to get yourself - and your friends - out of a tight spot.', false);
values('32', 'Helper', 'You love working as a part of a team and helping others reach their goals.', false);
values('33', 'Historian', 'You know a lot about the past, including both human history and ancient magicks.', false);
values('34', 'Hunting', 'You\'re a skilled mouser and enjoy stalking things.', false);
values('35', 'Investigation', 'You love a good mystery. Finding all the clues and then figuring out how they fit together is like licking up a bowl of cream.', false);
values('36', 'Musical', 'You are talented at singing and playing musical instruments.', false);
values('41', 'Naturalist', 'You know a lot about nature and are good at growing things.', false);
values('42', 'Navigator', 'You hardly ever get lost and you know how to find your way from here to there, wherever that happens to be.', false);
values('43', 'Night Vision', 'The night is your friend. You can easily see hwere you\'re going, even in total darkness.', false);
values('44', 'Planner', 'You think ahead, and can come up with a strategy for any situation.', false);
values('45', 'Puzzler', 'You enjoy solving problems, answering tricky riddles, decoding messages, and untangling baffling mysteries.', false);
values('46', 'Quick Reflexes', 'You react quickly, particularly when you\'re threatened.', false);
values('51', 'Reader', 'Human language comes easily to you, and you enjoy reading books and other writing.', false);
values('52', 'Runt of the Litter', 'You\'re smaller than most other kitties, so you can squeeze into places where they can\'t follow.', false);
values('53', 'Scientific', 'You know a lot about science, including chemistry and physics.', false);
values('54', 'Scrounging', 'You\'ve always been good at digging up the stuff you need, including food and shelter.', false);
values('55', 'Sense of Hearing', 'You have sharp ears, and can easily hear things others miss.', false);
values('56', 'Sense of Smell', 'You have a keen sense of smell and can identify things you\'ve seen before.', false);
values('61', 'Sense of Vision', 'You can see things far away and are good at spotting things out of the corner of your eye.', false);
values('62', 'Sleight of Paw', 'You make small objects vanish and appear as if by magic, but it\'s really because you\'re so quick with your paws.', false);
values('63', 'Snaring', 'You\'ve got a knack for setting snares and other traps.', false);
values('64', 'Sneaky', 'You\'re good at moving quietly and hiding.', false);
values('65', 'Tinkerer', 'You know how human machines work. You can easily work with technology and cobble together your own inventions.', false);
values('66', 'Trouble Seeker', 'You\'re good at finding and dealing with the supernatural troubles that bother magical kitties.', false);

commit;