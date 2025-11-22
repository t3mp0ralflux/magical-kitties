create table if not exists account (
    id UUID primary key,
    first_name text not null,
    last_name text not null,
    username text not null,
    password text not null,
    email text not null,
    created_utc timestamp not null,
    updated_utc timestamp not null,
    activated_utc timestamp null,
    last_login_utc timestamp null,
    deleted_utc timestamp null,
    account_status numeric not null,
    account_role numeric not null,
    activation_expiration timestamp null,
    activation_code text null,
    password_reset_requested_utc timestamp null,
    password_reset_code text null,
	UNIQUE (username, email)
);

create table if not exists refreshtoken (
    id UUID primary key,
    account_id UUID references account(id),
    access_token text not null,
    token text not null,
    expiration_utc timestamp not null,
    UNIQUE(account_id)
);

create table if not exists flaw(
    id numeric primary key,
    name text not null,
    description text not null,
    is_custom bool not null
);

create table if not exists talent(
    id numeric primary key,
    name text not null,
    description text not null,
    is_custom bool not null
);

create table if not exists magicalpower(
    id numeric primary key,
    name text not null,
    description text not null,
    is_custom bool not null,
    bonusfeatures json not null
);

create table if not exists character(
    id UUID primary key,
    account_id UUID references account(id),
    name text not null constraint max_description check (char_length(name) <= 100),
    username text not null,
    created_utc timestamp not null,
    updated_utc timestamp not null,
    deleted_utc timestamp null,
    description text not null constraint max_description check (char_length(description) <= 250),
    hometown text not null constraint max_description check (char_length(hometown) <= 100),
    upgrades json null
);

create table if not exists characterstat(
    id UUID primary key,
    character_id UUID references character(id),
    level numeric not null default 1,
    current_xp numeric not null default 0,
    max_owies numeric not null default 2,
    current_owies numeric not null default 0,
    starting_treats numeric not null default 2,
    current_treats numeric not null default 0,
    current_injuries numeric not null default 0,
    cute numeric not null default 0,
    cunning numeric not null default 0,
    fierce numeric not null default 0,
    incapacitated bool default false not null
);

create table if not exists characterflaw(
    id UUID primary key,
    character_id UUID references character(id),
    flaw_id numeric references flaw(id),
    UNIQUE(character_id, flaw_id)  
);

create table if not exists charactertalent(
    id UUID primary key,
    character_id UUID references character(id),
    talent_id numeric references talent(id),
    is_primary bool default false not null,
    UNIQUE(character_id, talent_id)
);

create table if not exists human(
    id UUID primary key,
    character_id UUID references character(id),
    name text not null constraint max_name check (char_length(name) <= 100),
    description text not null,
    deleted_utc timestamp null
);

create table if not exists problem(
    id UUID primary key,
    human_id UUID references human(id),
    source text not null constraint max_source check (char_length(source) <= 50),
    emotion text not null,
    rank numeric not null,
    solved bool not null,
    deleted_utc timestamp null
);

create table if not exists charactermagicalpower(
    id UUID primary key,
    character_id UUID references character(id),
    magical_power_id numeric references magicalpower(id),
    is_primary bool default false not null,
    selected_bonuses text,
    UNIQUE(character_id, magical_power_id)
);

create table if not exists upgradechoice(
    id UUID primary key,
    name text,
    upgrade_option numeric not null
);

create table if not exists upgraderule(
    id UUID primary key,
    block numeric not null,
    upgrade_choice UUID references upgradechoice(id),
    UNIQUE(block, upgrade_choice)
);

create table if not exists email(
    id UUID primary key,
    account_id_sender UUID references account(id),
    account_id_receiver UUID references account(id),
    should_send boolean not null,
    send_attempts int not null,
    sent_utc timestamp null,
    send_after_utc timestamp not null,
    sender_email varchar(50) not null,
    recipient_email varchar(50) not null,   
    body varchar not null,
    response_log varchar null
);

create table if not exists globalsetting (
    id UUID primary key,
    name varchar not null,
    value varchar not null
);