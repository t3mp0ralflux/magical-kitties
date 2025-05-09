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
values('545c30a9-f23d-4366-9c7f-10d7b24c4700', 'activation_email_format', '<p>Your activation link is:</p><p>{0}</p><p>For security reasons, do not share this link with anyone. This link will stop working after {1} minutes.</p><p>To keep future security messages like this from going to spam or junk email, Add no-reply@mail.magicalkitties.com to your approved or safe sender list.</p><p>If you did not make this request, contact us. Please do not respond to this email.</p>');

commit;

-- flaws
begin transaction;
insert into flaw(id, name, description, is_custom)
values('11', 'Amnesia', 'You are missing a part of your past, having lost some or all of your memory.', false);
insert into flaw(id, name, description, is_custom)
values('12', 'Arrogant', 'You think you can do anything, even when it''s clearly beyond your ability.', false);
insert into flaw(id, name, description, is_custom)
values('13', 'Big Mouth', 'You talk a lot and often say things when you shouldn''t, or reveal things that should be kept secret.', false);
insert into flaw(id, name, description, is_custom)
values('14', 'Careless', 'You are clumsy and inattentive. You often break things, perhaps because you don''t know your own strength.', false);
insert into flaw(id, name, description, is_custom)
values('15', 'Cry Baby', 'Anything remotely sad makes you break down in tears.', false);
insert into flaw(id, name, description, is_custom)
values('16', 'Distractable', 'You are easily sidetracked by shiny things.', false);
insert into flaw(id, name, description, is_custom)
values('21', 'Dizziness', 'Sometimes, things seem to spin around you when they shouldn''t.', false);
insert into flaw(id, name, description, is_custom)
values('22', 'Forgetful', 'Often, you don''t remember important information.', false);
insert into flaw(id, name, description, is_custom)
values('23', 'Gluttonous', 'You really like food. Even when you''ve just eaten, you find it hard to resist as quick bite.', false);
insert into flaw(id, name, description, is_custom)
values('24', 'Greedy', 'You want things, and you think you deserve more stuff than everyone else.', false);
insert into flaw(id, name, description, is_custom)
values('25', 'Grumpy', 'You are often in a bad mood for no reason.', false);
insert into flaw(id, name, description, is_custom)
values('26', 'Gullible', 'You are too innocent and trusting. You believe just about anythign somebody tells you.', false);
insert into flaw(id, name, description, is_custom)
values('31', 'Hallucinations', 'You often see or hear things that are not really there.', false);
insert into flaw(id, name, description, is_custom)
values('32', 'Hyperactive', 'You are full of boundless energey, and aren''t satisfied unless you''re doing something.', false);
insert into flaw(id, name, description, is_custom)
values('33', 'Impulsive', 'You tend to rush into things without thinking them through.', false);
insert into flaw(id, name, description, is_custom)
values('34', 'Indecisive', 'You have a hard time making up your mind, and then worry you made the wrong decision.', false);
insert into flaw(id, name, description, is_custom)
values('35', 'Jumps to Conclusions', 'You often assume you know what''s going on, even when you don''t.', false);
insert into flaw(id, name, description, is_custom)
values('36', 'Kittylexia', 'You can''t read human languages at all.', false);
insert into flaw(id, name, description, is_custom)
values('41', 'Lazy', 'You always prefer taking the easy way, or making someone else work for you.', false);
insert into flaw(id, name, description, is_custom)
values('42', 'Loud', 'You make a lot of noise, often at the worst possible time.', false);
insert into flaw(id, name, description, is_custom)
values('43', 'Nosey', 'You havea thirst for knowledge and are eager to investigate anything unusual, even if this annoys others.', false);
insert into flaw(id, name, description, is_custom)
values('44', 'Obsessive Collector', 'There is a specific item (or type of item) that you simply _must_ have.', false);
insert into flaw(id, name, description, is_custom)
values('45', 'Overactive Imagination', 'You frequently imagine things to be far different than what they really are.', false);
insert into flaw(id, name, description, is_custom)
values('46', 'Paranoid', 'You worry that everyone is plotting against you.', false);
insert into flaw(id, name, description, is_custom)
values('51', 'Pessimistic', 'You see the worst in everything. You know god times can''t last, and bad times always get worse.', false);
insert into flaw(id, name, description, is_custom)
values('52', 'Scaredy Cat', 'You are mildly afraid of just about everything, or extremely afraid of one specific thing.', false);
insert into flaw(id, name, description, is_custom)
values('53', 'Secrets', 'You have a really important secret (or many secrets) that you are keeping from your friends.', false);
insert into flaw(id, name, description, is_custom)
values('54', 'Show-Off', 'You always feel like you need to prove how awesome you are.', false);
insert into flaw(id, name, description, is_custom)
values('55', 'Shy', 'You feel awkward around other people, and have a hard time talking to strangers.', false);
insert into flaw(id, name, description, is_custom)
values('56', 'Sleepy', 'You really like taking naps, and sleep whenever you have the choice.', false);
insert into flaw(id, name, description, is_custom)
values('61', 'Snobby', 'You are conceited, always looking down on people who are not as good as you.', false);
insert into flaw(id, name, description, is_custom)
values('62', 'Soft-Hearted', 'You feel compassion for those in danger or discomfort, and are willing to help them in any way you can.', false);
insert into flaw(id, name, description, is_custom)
values('63', 'Squeamish', 'You feel faint or sick when confronted with anything that''s totally gross.', false);
insert into flaw(id, name, description, is_custom)
values('64', 'Stubborn', 'Your dogged determination means you refuse to change your mind.', false);
insert into flaw(id, name, description, is_custom)
values('65', 'Superstitious', 'You believe in myths, urban legends, and bad luck.', false);
insert into flaw(id, name, description, is_custom)
values('66', 'Vanity', 'You are very proud of your appearance. Grooming and presentation are very important to you.', false);

commit;

-- talents
begin transaction;
insert into talent(id, name, description, is_custom)
values('11', 'Animal Friend', 'You like and get long with squirrels, robins, and other non-magical critters. Even dogs, most of the time.', false);
insert into talent(id, name, description, is_custom)
values('12', 'Artistic', 'You are good at visual arts like painting and sculpting.', false);
insert into talent(id, name, description, is_custom)
values('13', 'Athletic', 'You are good at physical activities like running, jumping, and climbing.', false);
insert into talent(id, name, description, is_custom)
values('14', 'Balance', 'You have a great sense of balance and can easily walk along a tightrope, window ledge, or tree branch.', false);
insert into talent(id, name, description, is_custom)
values('15', 'Bargainer', 'You''re good at getting what you want, but even more importantly you enjoy helping people who are arguing come to an agreement.', false);
insert into talent(id, name, description, is_custom)
values('16', 'Big Kitty', 'You''re larger than most other kitties, and you can throw your weight around.', false);
insert into talent(id, name, description, is_custom)
values('21', 'Calculator', 'You understand numbers and math easily, and you like applying them to real life.', false);
insert into talent(id, name, description, is_custom)
values('22', 'Claws', 'You are very proud of your razor-sharp claws, and can use them in all sorts of clever ways.', false);
insert into talent(id, name, description, is_custom)
values('23', 'Dancer', 'You know how to move to music and get your groove on. Your dancing looks good, and it''s fun, too!', false);
insert into talent(id, name, description, is_custom)
values('24', 'Daredevil', 'You love taking really big risks and leaping into danger.', false);
insert into talent(id, name, description, is_custom)
values('25', 'Dramatist', 'You''re a talented actor who can spin a good story that entertains others.', false);
insert into talent(id, name, description, is_custom)
values('26', 'Empathic', 'You have a knack for understanding the feelings of others.', false);
insert into talent(id, name, description, is_custom)
values('31', 'Escape Artist', 'You know how to get yourself - and your friends - out of a tight spot.', false);
insert into talent(id, name, description, is_custom)
values('32', 'Helper', 'You love working as a part of a team and helping others reach their goals.', false);
insert into talent(id, name, description, is_custom)
values('33', 'Historian', 'You know a lot about the past, including both human history and ancient magicks.', false);
insert into talent(id, name, description, is_custom)
values('34', 'Hunting', 'You''re a skilled mouser and enjoy stalking things.', false);
insert into talent(id, name, description, is_custom)
values('35', 'Investigation', 'You love a good mystery. Finding all the clues and then figuring out how they fit together is like licking up a bowl of cream.', false);
insert into talent(id, name, description, is_custom)
values('36', 'Musical', 'You are talented at singing and playing musical instruments.', false);
insert into talent(id, name, description, is_custom)
values('41', 'Naturalist', 'You know a lot about nature and are good at growing things.', false);
insert into talent(id, name, description, is_custom)
values('42', 'Navigator', 'You hardly ever get lost and you know how to find your way from here to there, wherever that happens to be.', false);
insert into talent(id, name, description, is_custom)
values('43', 'Night Vision', 'The night is your friend. You can easily see hwere you''re going, even in total darkness.', false);
insert into talent(id, name, description, is_custom)
values('44', 'Planner', 'You think ahead, and can come up with a strategy for any situation.', false);
insert into talent(id, name, description, is_custom)
values('45', 'Puzzler', 'You enjoy solving problems, answering tricky riddles, decoding messages, and untangling baffling mysteries.', false);
insert into talent(id, name, description, is_custom)
values('46', 'Quick Reflexes', 'You react quickly, particularly when you''re threatened.', false);
insert into talent(id, name, description, is_custom)
values('51', 'Reader', 'Human language comes easily to you, and you enjoy reading books and other writing.', false);
insert into talent(id, name, description, is_custom)
values('52', 'Runt of the Litter', 'You''re smaller than most other kitties, so you can squeeze into places where they can''t follow.', false);
insert into talent(id, name, description, is_custom)
values('53', 'Scientific', 'You know a lot about science, including chemistry and physics.', false);
insert into talent(id, name, description, is_custom)
values('54', 'Scrounging', 'You''ve always been good at digging up the stuff you need, including food and shelter.', false);
insert into talent(id, name, description, is_custom)
values('55', 'Sense of Hearing', 'You have sharp ears, and can easily hear things others miss.', false);
insert into talent(id, name, description, is_custom)
values('56', 'Sense of Smell', 'You have a keen sense of smell and can identify things you''ve seen before.', false);
insert into talent(id, name, description, is_custom)
values('61', 'Sense of Vision', 'You can see things far away and are good at spotting things out of the corner of your eye.', false);
insert into talent(id, name, description, is_custom)
values('62', 'Sleight of Paw', 'You make small objects vanish and appear as if by magic, but it''s really because you''re so quick with your paws.', false);
insert into talent(id, name, description, is_custom)
values('63', 'Snaring', 'You''ve got a knack for setting snares and other traps.', false);
insert into talent(id, name, description, is_custom)
values('64', 'Sneaky', 'You''re good at moving quietly and hiding.', false);
insert into talent(id, name, description, is_custom)
values('65', 'Tinkerer', 'You know how human machines work. You can easily work with technology and cobble together your own inventions.', false);
insert into talent(id, name, description, is_custom)
values('66', 'Trouble Seeker', 'You''re good at finding and dealing with the supernatural troubles that bother magical kitties.', false);

commit;

-- magical powers
begin transaction;
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('11', 'Alter Body', 'You can change your body into any one substance. Choose the material when you take this Magical Power. You keep your size and shape, and can move as normal, but get the substance''s other qualities like hardness and weight. At first you can only mimic a solid, like wood or stone or metal. Things you''re carrying don''t change with you.', false, '[{"id": 1, "name": "Any Solid Body", "description":"You can turn into any solid material you like, rather than being limited to one, if you touch a sample of the material you want to copy.", "is_custom":false, "selected":false},{"id": 2, "name": "Liquid Body", "description":"You can turn your body into any liquid, like water, tree sap, or milk. You aren''t limited to one, but you must have a sample to touch.", "is_custom":false, "selected": false},{"id": 3, "name": "Gaseous Body", "description":"You can turn your body into a kitty-sized cloud of gas. This gas is non-toxic and visible to others. While acting as a gas cloud you''ll generally look like yourself, but you can also squeeze through any gap that isn''t airtight.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('12', 'Bouncing', 'When jumping or being thrown against surfaces, you bounce off like a rubber ball. You might roll up into a ball or blow up like a balloon to help make this happen. This won''t stop you from getting hurt if you fall from an extreme height or are thrown with Super Strength, but it does prevent Owies from normal bounces.', false, '[{"id": 1, "name": "Endless Fall", "description":"You can fall from any height, or be thrown with any amount of force and bounce off the ground without suffering an Owie or Injury. This means the GM can''t inflict an Owie as a complication from falling or being thrown, even if you roll a failure when using your power.", "is_custom":false, "selected":false},{"id": 2, "name": "Hyper-Bouncing", "description":"In addition to simply bouncing off a surface, you actually gain speed as you bounce. The more you bounce, the faster and faster you go. You can''t go quite as fast as someone with Super Speed power, but if you can line up enough bounces you can get pretty close", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('13', 'Burrowing', 'You can tunnel through the ground as fast as you can walk. You can only burrow through dirt of similar substances, digging like a worm and pushing back material that closes up the tunnel behind you, leaving no trace.', false, '[{"id": 1, "name": "Open Tunnel", "description":"While burrowing you can choose to leave an open tunnel behind you, allowing others to follow.", "is_custom":false, "selected":false},{"id": 2, "name": "Rock Digger", "description":"In addition to dirt, you can also tunnel through solid rock and concrete.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('14', 'Catfish', 'You are completely amphibious, able to breathe underwater and swim as effortlessly as other kitties walk. You may even grow a fish tail when you touch water, or have one all the time.', false, '[{"id": 1, "name": "Super Swimmer", "description":"You can swim as fast as a kitty with the Super Speed power can run. This only works in water, but you can also select Super Speed bonus featuers (22) to improve your Catfish super swimming.", "is_custom":false, "selected":false},{"id": 2, "name": "Water Warping", "description":"You can mentally command water to move as you will it. You can control a small amount of water (as much as would fit in a bucket), but it can float through the air and move very quickly in response to your commands.", "is_custom":false, "selected":false},{"id": 3, "name": "Advanced Water Warping", "description":"You can command large bodies of water, up to the size of a swimming pool. (You must take Water Warping first.)", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('15', 'Copycat', 'You can create one perfect duplicate of yourself. This copy shares all your memories and experiences. You may actually disagree about which of you is the duplicate and which is the original. If you''re touching your copy, you can merge back together, gaining all of the experiences your duplicate had while separated from you.<br/>If you have Owies or Injuries befoer you duplicate, your duplicate is also affected by those wounds. If copies have different numbers of Owies/Injuries when merging back together, the merged version has Owies/Injuries equal to whichever duplicate had the _fewest_ Owies/Injuries.</p>', false, '[{"id": 1, "name": "We Are Many", "description":"You can create up to six duplicates simultaneously.", "is_custom":false, "selected":false},{"id": 2, "name": "I Am Legion", "description":"There is no limit to the number of copies you can create. (You must take We Are Many first.)", "is_custom":false, "selected":false},{"id": 3, "name": "Mindlink", "description":"You and all your duplicates share a telepathic link. You know where the are at all times (and vice versa), and you can also telepathically speak with each other freely.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('16', 'Detector', 'You can find any specific object or type of object that you want, as long as it''s within fifty (50) feet of you. Detecting feels like playing hot-and-cold with your own mind.', false, '[{"id": 1, "name": "Long-Range Detecting", "description":"Your power can detect objects as long as they''re within one mile of you.", "is_custom":false, "selected":false},{"id": 2, "name": "Limitless Detecting", "description":"There''s no limit to the distance at which you can detect an object. (You must take Long-Range Detecting first.)", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('21', 'Dreamwalker', 'By entering a trance-like state, your kitty can watch the dream of one sleeping person they know or can currently see.', false, '[{"id": 1, "name": "Dream Interaction", "description":"As well as simply watching a dream, the Dreamwalker can now enter that dream and interact with the dreamer, and the creations of the dreamer''s mind.", "is_custom":false, "selected":false},{"id": 2, "name": "Dream Companions", "description":"The Dreamwalker can let others who are touching them observe or enter the dream along with them. Their dream-selves don''t need to keep touching each other, only their physical bodies have to stay in contact.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('22', 'Energy Deflection', 'When energy attacks, like lightning bolts or laser beams, would normally hit you, you an instead deflect them harmlessly away. You can also deflect energy sources that you touch, like beams of energy.', false, '[{"id": 1, "name": "Energy Reflection", "description":"As well as deflecting energy, you can also reflect them back to their source.", "is_custom":false, "selected":false},{"id": 2, "name": "Energy Redirection", "description":"You can choose to aim our Energy Reflection not at the source of the effect, but at another target you can see, instead. (You must take Energy Reflection first.)", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('23', 'Force Field', 'You can surround yourself with a bubble of force that moves with you and stops anything from penetrating it. At first, the Force Field is big enough to protect yourself and a few friends. A Force Field won''t protect you from things that don''t need to physically reach you, like a witch''s sleep spell or a hypnotist''s mesmerizing gaze. It also won''t stop anything that can''t be stopped by a physical barrier, like a ghost that can walk through walls.', false, '[{"id": 1, "name": "Big Force Field", "description":"The bubble of force you create is big enough to protect a house.", "is_custom":false, "selected":false},{"id": 2, "name": "Magical Sphere", "description":"Your Force Field glows with magical runes and prevents all magical effects and supernatural spirits from passing through or affecting those within the Force Field.", "is_custom":false, "selected":false},{"id": 3, "name": "Wall of Force", "description":"You can use your Force Field to create a moveable, realizable Wall of Force instead of just a bubble surrounding you. If you make this wall horziontal, it can be a platform that you or others can walk across.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('24', 'Frost Breath', 'With this power, your kitty breathes out a strong gust of freezing wind. At first, the wind is powerful enough to knock over small objects or send papers scattering through the air, and it''s cold enough the freeze room-temperature liquids and cause humans to shiver.', false, '[{"id": 1, "name": "Freeze Breath", "description":"The kitty can also breathe so that a layer of ice covers people and animals, freezing them in place for the scene.", "is_custom":false, "selected":false},{"id": 2, "name": "Knock-Back Breath", "description":"The wind is so strong that it sends creatures and objects as big as a horse flying through the air.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('25', 'Flight', 'Your kitty can fly, whether because you have wings or can magically levitate above the ground. While flying, you can carry as much as while walking.', false, '[{"id": 1, "name": "Share Flight", "description":"You can allow other friends who are near you to fly along with you, whether by carrying them or just staying within your magical range.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('26', 'Healer', 'You can use your Magical Power to remove one Owie or Injury from another kitty or yourself.', false, '[{"id": 1, "name": "Long-Term Care", "description":"Whenever a kitty would normally recover one Owie or Injury, you can make a Cunning check (using your power''s +2 dice) at difficulty 2+ the number of Injuries they''re suffering. If you succeed, the can insterad remove two Owies or Injuries.", "is_custom":false, "selected":false},{"id": 2, "name": "Mass Healing", "description":"Instead of only being able to heal just one kitty per scene, you can heal both yourself and _all_ of your friends in the scene.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('31', 'Hypnosis', 'You can put others in a tranced that makes them very suggestible. At first, the power only works on animals. You can only hypnotize one animal at a time who can hear you, and you can''t make them do anything that''s obviously dangerous like jumping off a cliff.', false, '[{"id": 1, "name": "Human Hypnosis", "description":"You can hypnotize a human. While under hypnosis, the human will obey your commands and won''t remember you talking afterwards.", "is_custom":false, "selected":false},{"id": 2, "name": "Dangerous Hypnosis", "description":"You can hypnotize a creature to do dangerous things.", "is_custom":false, "selected":false}, {"id": 3, "name": "Mass Hypnosis", "description":"You can hypnotize anybody who can hear you, all at the same time.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('32', 'Illusion', 'Your kitty can create moving images that can fool other creatures and humans into thinking they''re real. At first, these illusions are limited to an image of a single object or creature, and you have to be able to see the illusion to keep it going. Illusions are visual projections, like a movie on a theater screen, that can''t be touched, smelled, or heard.', false, '[{"id": 1, "name": "Persistent Images", "description":"The illusions created by your kitty no longer need your attention. You can create an illusion and walk away, leaving the image in place behind you.", "is_custom":false, "selected":false},{"id": 2, "name": "Full-Sensory Illusions", "description":"In addition to sight, your illusions produce sound and fool other senses like hearing, taste, and smell. They are still insubstantial, however, and can''t be touched.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('33', 'Invisibility', 'You can turn invisible. Nobody can see you, but they can still hear, smell, and touch you. Objects you wear or carry are still visible.', false, '[{"id": 1, "name": "Share Invisibility", "description":"You can also make friends near you invisible.", "is_custom":false, "selected":false}, {"id": 2, "name": "Object Invisibility", "description":"You can turn any object you touch invisible.", "is_custom":false, "selected":false}, {"id": 3, "name": "See Invisibility", "description":"You can see other creatures who are invisible.", "is_custom":false, "selected":false}, {"id": 4, "name": "Soundless", "description":"In addition to being invisible, you also make no sound.", "is_custom":false, "selected":false}, {"id": 5, "name": "Scentless", "description":"In addition to being invisible, you also can''t be detected by smell.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('34', 'Laser Eyes', 'You shoot lasers out of your eyes. You can change the strength from simply making a dot of light, to being able to slowly cut through wood.', false, '[{"id": 1, "name": "Laser Cutter", "description":"Your laser eyes can quickly cut through any non-living material, even metal.", "is_custom":false, "selected":false},{"id": 2, "name": "Laser Scanner", "description":"Your laser eyes can process information from laser-based mediums used by humans, like barcodes, DVDs, and CDs", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('35', 'Mind Transfer', 'You can project your mind into that of a creature (but not a human) that you can see and take control of their body. You can only control one creature at a time, while your own body lies helpless.', false, '[{"id": 1, "name": "Human Transfer", "description":"You can take control of humans.", "is_custom":false, "selected":false}, {"id": 2, "name": "Mental Projection", "description":"You don''t need to be able to see a subject to take control of their body. You can mentally scan for creatures within a city block of you, and try to control any creature in that range.", "is_custom":false, "selected":false}, {"id": 3, "name": "Mob Mentality", "description":"You can split your consciousness, taking control of as many as six different creatures at the same time.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('36', 'Nullify', 'You can cancel the Magical Power of a Foe, another kitty, a magical object, or a technology powered by magic. This includes ending an ongoing effect, or also keeping a Magical Power from taking effect.', false, '[{"id": 1, "name": "Block Powers", "description":"As well as nullifying a Magical Power, you can prevent a kitty, Foe, object, or technology from using the power for the next 24 hours.", "is_custom":false, "selected":false},{"id": 2, "name": "Power Thief", "description":"After nullifying a Magical Power, you can immediately try to use that power yourself. You must succeed at another Attribute check to do this.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('41', 'Phasing', 'You can walk through solid objects. You can only take yourself, not friends or objects you carry.', false, '[{"id": 1, "name": "Share Phasing", "description":"You can let nearby friends phase along with you.", "is_custom":false, "selected":false}, {"id": 2, "name": "Carry Phasing", "description":"You can carry as many objects with you while you phase as your can normally.", "is_custom":false, "selected":false}, {"id": 3, "name": "Reactive Phasing", "description":"You can phase in reaction to something coming at you. For example, bullets pass right through you.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('42', 'Pyrokinesis', 'You can create and control fires with your mind. At first, you can control as much as a single campfire. The fire still needs a source of fuel, like wood or oil.', false, '[{"id": 1, "name": "Fireball", "description":"You can throw balls of fire even if there is no fuel.", "is_custom":false, "selected":false},{"id": 2, "name": "All the Flames", "description":"You can control as much fire as you can see.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('43', 'Shadow Form', 'You can merge your kitty''s body into its own shadow. The shadow can''t physically interact with things or creatures, but it can travel freely anywhere a shadow might go, including under doors or through windows. Your kitty can re-emerge from your shadow at any time, as long as there''s space to do it.', false, '[{"id": 1, "name": "Separate Shadow", "description":"You can detach your kitty''s shadcow and send it out on its own. The shadow can follow simple commands, but it doesn''t have the creativity and independence of a kitty.", "is_custom":false, "selected":false}, {"id": 2, "name": "Control Shadows", "description":"Your kitty can also detach and control the shadows of others. (You must take the Separate Shadow bonus feature first.)", "is_custom":false, "selected":false}, {"id": 3, "name": "Shadow Force", "description":"Your shadow form and any other shadows you control can interact with the shadows of things or creatures as if they had physical form. For example, you can try to push someone over by having your shadow push their shadow.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('44', 'Shapechanging', 'You can change into other creatures or things. At first, you can turn into any land animal that''s from half your size to double your size. No matter how skilled you become, you can never turn into a human (even though, yes, humans are technically animals).', false, '[{"id": 1, "name": "Flying Animals", "description":"You can turn into any flying animal.", "is_custom":false, "selected":false}, {"id": 2, "name": "Swimming Animals", "description":"You can turn into any swimming animal", "is_custom":false, "selected":false}, {"id": 3, "name": "Inanimate Objects", "description":"You can turn into any inanimate object, like a rock or a table.", "is_custom":false, "selected":false}, {"id": 4, "name": "Big", "description":"You can be as big as an elephant. IKtems and clothing you''re carrying don''t grow with you.", "is_custom":false, "selected":false}, {"id": 5, "name": "Small", "description":"You can be as small as a flea. Items and clothing you''re carrying don''t shrink with you.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('45', 'Sight Beyond Sight', 'You can see visions of things that are happening far off. At first, you can see things as they''re happening right now, within the same city or similar area.', false, '[{"id": 1, "name": "See Past", "description":"You can see into the past.", "is_custom":false, "selected":false}, {"id": 2, "name": "See Future", "description":"You can see into the future.", "is_custom":false, "selected":false}, {"id": 3, "name": "See Anywhere", "description":"You can see things that are anywhere in the same universe as you.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('46', 'Size Master', 'You can reduce or increase your size. You can shrink yourself down to the size of a hamster, or grow up to the size of a tiger. Things you''re carrying or wearing don''t shrink or grow with you.', false, '[{"id": 1, "name": "Extreme Size", "description":"You can be as big as an elephant or as small as a flea.", "is_custom":false, "selected":false}, {"id": 2, "name": "Small and Strong", "description":"No matter how tiny you become, you remain as strong as you would as a full-grown kitty.", "is_custom":false, "selected":false}, {"id": 3, "name": "Take It With You", "description":"Objects you''re carrying or wearing now shrink and grow with you, if you want them to.", "is_custom":false, "selected":false}, {"id": 4, "name": "Share Size", "description":"You can also make friends who are nearby shrink or grow with you.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('51', 'Sound Master', 'You can create noises and make things sound like things they''re not. At first, these sounds have to mimic nature, and can be no louder than an elephant trumpeting.', false, '[{"id": 1, "name": "Imitate Machines", "description":"You can mimic the sounds of machines and other man-made sounds.", "is_custom":false, "selected":false}, {"id": 2, "name": "Imitate Speech", "description":"You can mimic a particular human''s voice", "is_custom":false, "selected":false}, {"id": 3, "name": "Sonic Boom", "description":"You can create sound so loud that it can shatter objects and shake people.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('52', 'Stretching', 'You can stretch out any part of your body you choose, including your neck, torso, legs, or tail. You can stretch them nearly as long as a grown-up human is tall.', false, '[{"id": 1, "name": "Super Stretching", "description":"You can stretch as far as a house is tall.", "is_custom":false, "selected":false},{"id": 2, "name": "Stretchy Sheet", "description":"You can stretch your body into a thin sheet, which works as a trampoline, parachute, and other useful things.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('53', 'Super Senses', 'Your kitty''s senses are incredibly powerful, letting you detect things other kitties can''t. At first, this is limited to super vision. Your kitty can \"zoom in\" to see things at almost any distance or at microscopic scale. You also have infrared vision and night vision, letting you see even in the dark.', false, '[{"id": 1, "name": "X-Ray Vision", "description":"Your kitty can also see through walls. X-ray vision can be blocked by thin sheets of lead or similar materials.", "is_custom":false, "selected":false}, {"id": 2, "name": "Super Hearing", "description":"All kitties can hear things at very high and very low frequencies, like dog whistles, that humans can''t. But with Super Hearing you can also hear things at far distances, like people talking quietly several rooms away.", "is_custom":false, "selected":false}, {"id": 3, "name": "Super Smell", "description":"You can follow a trail that could be days or even weeks old, depending on conditions. You can also detect smells at far distances.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('54', 'Super Speed', 'Your kitty can run really fast. At first, you can run just under the speed of sound. You can carry as much as your normally could, but can''t carry other kitties.', false, '[{"id": 1, "name": "Hop on Board", "description":"You can carry other kitties or animals while you run, but no more than you could normally carry. Under normal circumstances, this might be one other kitty, or a small animal in your mouth and one or two more on your back.", "is_custom":false, "selected":false},{"id": 2, "name": "Blink of an Eye", "description":"Yo ucan run almost at the speed of light, so fast that others see you merely as a blur.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('55', 'Super Strength', 'You can lift far more than a regular kitty. You''re able to easily pick up anything weighing as much as a horse or less.', false, '[{"id": 1, "name": "Pick Up a Whale", "description":"You can lift several hundred tons, enough to pick up a blue whale.", "is_custom":false, "selected":false},{"id": 2, "name": "Pick Up Anything", "description":"You can pick up anything that you can get your paws on. (You have to take Pick Up a Whale first.)", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('56', 'Technocat', 'You control machines with your thoughts. At first, you can operate electronic machines, like computers or radio-controlled cars. You can make them do anything a human could normally make them do. If the machine is intelligent, like a robot, you can talk to it with your mind. At first you can only control one machine at a time.', false, '[{"id": 1, "name": "All the Machines", "description":"You can control as many machines as you can see.", "is_custom":false, "selected":false}, {"id": 2, "name": "Any Machine", "description":"You can control any kind of machine, even if they are not normally operated electronically.", "is_custom":false, "selected":false}, {"id": 3, "name": "Advanced Control", "description":"You can make machines do things they can''t normally do, like having a home computer press its own keyboard or a light bulb change its color.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('61', 'Telekinesis', 'Telekinesis lets you move things by just thinking about it. At first, kitties with this power can only lift one small object, but as they gain experience they can lift more and heavier objects.', false, '[{"id": 1, "name": "Heavy", "description":"You can lift an object as heavy as a horse.", "is_custom":false, "selected":false},{"id": 2, "name": "Multiple Objects", "description":"You can lift as many things as you can see at one time.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('62', 'Telepathy', 'You can eavesdrop on others'' thoughts, including humans. You don''t control their thoughts, and you can''t force them to think of particular things, at least not with your Magical Power. At first you can only observe whatever they''re currently thinking about.', false, '[{"id": 1, "name": "Mind Probe", "description":"You are no longer limited to observing what your target is currently thinking about. You can delve into their mind and pull out specific information you''re looking for.", "is_custom":false, "selected":false},{"id": 2, "name": "Mindlink", "description":"You can use your telepathy to transmit throughts directly into other creatures'' minds. They \"hear\" your voice or see images you transmit. They know these thoughts aren''t their own, though. You can also permanently link willing minds together, letting them all telepathically communicate with each other.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('63', 'Teleport', 'You can instantly travel from one place to another without passing through the space between. At first, this power is limited to a spot in the same room as you that you can see, or within roughly twenty(20) feet.', false, '[{"id": 1, "name": "Teleport to the Limits of Sight", "description":"You can teleport to any place you can see, including that mountain way off in the distance or the plane high above your head.", "is_custom":false, "selected":false}, {"id": 2, "name": "Teleport Without Limit", "description":"You can teleport to any place you know. (You must take Teleport to the Limits of Sight first.)", "is_custom":false, "selected":false}, {"id": 3, "name": "Dimensional Teleport", "description":"You can teleport to other dimensions or planes of reality, but you have to be familiar with them already. (You must take Teleport Without Limit first.)", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('64', 'Time Freeze', 'You can stop time for everybody but you for about a minute. While time is stopped, you can''t directly affect other creatures or move objects except those you were touching when you froze time.', false, '[{"id": 1, "name": "Unfreeze", "description":"While time is frozen, you can move objects and unfreeze other creatures by touching them.", "is_custom":false, "selected":false},{"id": 2, "name": "Long Freeze", "description":"You can freeze time for up to an hour.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('65', 'Undead', 'You exist halfway between life and death. You might be a zombie, a vampire, or some other form of undead that still has a physical body. You are also driven by an unnatural hunger. Vampires, for example, thirst for blood. Zombies crave brains. More-exotic undead might feed on moonlight or a strong emotion. Undead only recover from Injuries if they''re able to sate their unnatural hunger. At first, you can''t create other undead.', false, '[{"id": 1, "name": "Minions", "description":"You can create undead minions similar to yourself. These minions generally follow your commands, but sometimes get ideas of their own. Choose a secret cure for your minions.", "is_custom":false, "selected":false},{"id": 2, "name": "Supernatural Feat", "description":"Pick another Magical Power to be the Supernatural Feat made possible by your undead form. This can only be done once, but the selected Magical Power can be boosted with its own Bonus Features.", "is_custom":false, "selected":false}]');
insert into magicalpower(id, name, description, is_custom, bonusfeatures)
values('66', 'Zap', 'You''re able to control electricity. You can create an annoying static shock, shoot small lightning bolts from your paws, or provide power to a single small appliance.', false, '[{"id": 1, "name": "Blackout", "description":"You can cause an area up to the size of a city to lose power.", "is_custom":false, "selected":false},{"id": 2, "name": "Dynamo", "description":"You can give power to several objects, or as many appliances as you might find in a single home.", "is_custom":false, "selected":false}]');

commit;

-- upgrade choices
begin transaction;

insert into upgradechoice(id, name)
values('d54036bb-a755-4d86-8774-78715bbf1d30', 'Gain a Bonus Feature for a Magical Power you have');
insert into upgradechoice(id, name)
values('6a244a6e-5fd9-4574-93e1-78193c7d85b6', 'Improve 1 Attribute (Max 3)');
insert into upgradechoice(id, name)
values('25b47167-600d-41c0-ad78-a404295d9bd8', 'Improve 1 Attribute (Max 4)');
insert into upgradechoice(id, name)
values('7712d17b-e553-402c-8467-4d9b2389956b', 'Increase Owie Limit +1');
insert into upgradechoice(id, name)
values('490b8218-01a7-4949-b993-9ce73061e749', 'Increase Kitty Treats +1');
insert into upgradechoice(id, name)
values('84725926-e714-4fee-8143-a05d58a24589', 'Gain Talent');
insert into upgradechoice(id, name)
values('ac4bffa2-396b-437d-9ea8-cb2788edac5d', 'New Magical Power');

commit;

-- upgrade rules
begin transaction;

insert into upgraderule(id, block, upgradechoice)
values('dda028e5-f8e6-4f22-a9b0-efed0de91fbf', '1', 'd54036bb-a755-4d86-8774-78715bbf1d30');
insert into upgraderule(id, block, upgradechoice)
values('5e11dede-1478-4dbf-af98-ecba78cc7d53', '1', '6a244a6e-5fd9-4574-93e1-78193c7d85b6');
insert into upgraderule(id, block, upgradechoice)
values('0f25d541-85e4-4297-9b10-e6d47f0a1f53', '1', '7712d17b-e553-402c-8467-4d9b2389956b');
insert into upgraderule(id, block, upgradechoice)
values('b79ac91c-de56-44d5-b453-ece207c80690', '1', '490b8218-01a7-4949-b993-9ce73061e749');
insert into upgraderule(id, block, upgradechoice)
values('5e3915b0-f3eb-4735-849f-384d0a279b97', '2', '84725926-e714-4fee-8143-a05d58a24589');
insert into upgraderule(id, block, upgradechoice)
values('14db8501-62c8-458b-a374-e4e4f8b6544b', '2', 'd54036bb-a755-4d86-8774-78715bbf1d30');
insert into upgraderule(id, block, upgradechoice)
values('c4ce0ca2-e75c-4aa1-b794-095e54a7b9de', '2', '25b47167-600d-41c0-ad78-a404295d9bd8');
insert into upgraderule(id, block, upgradechoice)
values('a436397f-c3a2-4d8f-834e-6b85942d2dc8', '2', '7712d17b-e553-402c-8467-4d9b2389956b');
insert into upgraderule(id, block, upgradechoice)
values('6a14874f-9b9e-47d8-b1dc-d958b386ec40', '2', '490b8218-01a7-4949-b993-9ce73061e749');
insert into upgraderule(id, block, upgradechoice)
values('ce331a83-b83a-4f23-b170-03d87106fe9a', '3', 'ac4bffa2-396b-437d-9ea8-cb2788edac5d');
insert into upgraderule(id, block, upgradechoice)
values('5e1dce80-a7e0-4259-9474-0bca65f1ddeb', '3', 'd54036bb-a755-4d86-8774-78715bbf1d30');
insert into upgraderule(id, block, upgradechoice)
values('887daa20-a18e-46e8-839e-bedeb0ea1a57', '3', '25b47167-600d-41c0-ad78-a404295d9bd8');
insert into upgraderule(id, block, upgradechoice)
values('c532dc34-d6ef-48f1-96a9-a02604f1d609', '3', '7712d17b-e553-402c-8467-4d9b2389956b');
insert into upgraderule(id, block, upgradechoice)
values('34ba7894-b947-4764-8307-e21c729eacd0', '3', '490b8218-01a7-4949-b993-9ce73061e749');

commit;